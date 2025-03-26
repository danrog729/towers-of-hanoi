using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using towers_of_hanoi.Navigation.Multiplayer;

namespace towers_of_hanoi
{
    /// <summary>
    /// Interaction logic for Multiplayer.xaml
    /// </summary>
    public partial class Multiplayer : Page
    {
        Scene3D scene;
        Game localGame;
        Game remoteGame;
        Point lastMousePos;
        bool rightMouseDownLast;

        public int discCount = 6;
        public int poleCount = 3;
        float discHeight = 2;

        bool inGame;
        DispatcherTimer timer;
        Stopwatch stopwatch = new Stopwatch();

        public Multiplayer()
        {
            InitializeComponent();
            scene = new Scene3D(Viewport);
            localGame = new Game(poleCount, discCount, 0, 2);
            remoteGame = new Game(poleCount, discCount, 0, 2);
            scene.Reset(discCount, poleCount, 0, discHeight);
            lastMousePos = new Point(0, 0);
            rightMouseDownLast = false;

            inGame = false;
            timer = new DispatcherTimer();
            stopwatch = new Stopwatch();
            timer.Interval = TimeSpan.FromSeconds(0.001);
            timer.Tick += UpdateTimerText;
        }

        private void ViewportMouseMoved(object sender, MouseEventArgs e)
        {
            if (e.RightButton.Equals(MouseButtonState.Pressed))
            {
                // move camera
                System.Windows.Point currentPos = e.GetPosition(Viewport);
                if (rightMouseDownLast)
                {
                    float deltaX = (float)(currentPos.X - lastMousePos.X);
                    float deltaY = (float)(currentPos.Y - lastMousePos.Y);

                    scene.RotateCamera(deltaX * 0.005f, deltaY * 0.005f);
                }
                rightMouseDownLast = true;
                lastMousePos = currentPos;
            }
            else
            {
                rightMouseDownLast = false;
            }

            if (e.LeftButton.Equals(MouseButtonState.Pressed))
            {
                // potentially drag-dropping
                System.Windows.Point currentPos = e.GetPosition(Viewport);
                if (scene.ValidDragDrop)
                {
                    // get which pole the mouse is over, if any
                    int overPole = scene.MoveDragAndDrop(currentPos);
                    if (overPole != -1 && overPole != scene.DraggingLastOver && localGame.NumberOnPole(scene.DraggingFrom) != 0)
                    {
                        // hovering over a pole
                        scene.HoverDisc(localGame.PeekPole(scene.DraggingFrom), overPole);
                        scene.DraggingLastOver = overPole;
                    }
                }
            }
        }

        private void ViewportMouseScrolled(object sender, MouseWheelEventArgs e)
        {
            scene.ZoomCamera(e.Delta * 0.0005f);
        }

        private void ViewportLeftMouseDown(object sender, MouseEventArgs e)
        {
            // drag and drop
            Viewport.Focus();
            System.Windows.Point currentPos = e.GetPosition(Viewport);
            scene.SelectObjectForDragAndDrop(currentPos);
            if (scene.ValidDragDrop && localGame.NumberOnPole(scene.DraggingFrom) != 0)
            {
                scene.HoverDisc(localGame.PeekPole(scene.DraggingFrom), scene.DraggingFrom);
                scene.DraggingLastOver = scene.DraggingFrom;
                if (!inGame)
                {
                    inGame = true;
                    stopwatch.Start();
                    timer.Start();
                }
            }
        }

        private void ViewportLeftMouseUp(object sender, MouseEventArgs e)
        {
            // figure out the move thats just been played
            Viewport.Focus();
            System.Windows.Point currentPos = e.GetPosition(Viewport);
            (int, int) move = scene.ReleaseDragAndDrop(currentPos);

            MoveDisc(move);
        }

        private void ViewportKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 && Viewport.IsFocused)
            {
                int poleNumber = e.Key - Key.D1;
                if (poleNumber == -1) poleNumber = 9;
                if (poleNumber < poleCount)
                {
                    if (!scene.ValidDragDrop)
                    {
                        if (localGame.NumberOnPole(poleNumber) != 0)
                        {
                            scene.SelectDirectMove(poleNumber);
                            scene.HoverDisc(localGame.PeekPole(scene.DraggingFrom), scene.DraggingFrom);
                            if (!inGame)
                            {
                                inGame = true;
                                stopwatch.Start();
                                timer.Start();
                            }
                        }
                    }
                    else
                    {
                        MoveDisc((scene.DraggingFrom, poleNumber));
                        scene.ReleaseDirectMove();
                    }
                }
            }
        }

        public void Initialise()
        {
            TCP.MoveMessageReceived += ReceivedMove;
            TCP.LeaveMessageReceived += OtherPlayerLeft;
        }

        public void Deinitialise()
        {
            TCP.MoveMessageReceived -= ReceivedMove;
            TCP.LeaveMessageReceived -= OtherPlayerLeft;
        }

        private void OtherPlayerLeft(object? sender, EventArgs e)
        {
            // show a messagebox warning that the other player left, then go to singleplayer
            TCP.CloseServer();
            TCP.Disconnect();
            MessageBox.Show("The other player disconnected.");
            ((MainWindow)App.MainApp.MainWindow).SwitchToSingleplayer(discCount, poleCount);
        }

        private void ReceivedMove(object? sender, EventArgs e)
        {
            (int, int, string)? data = sender as (int, int, string)?;
            if (data != null)
            {
                (int, int, string) moves = data.Value;
                remoteGame.MoveDisc(moves.Item1, moves.Item2);
                if (remoteGame.GameWon && !localGame.GameWon)
                {
                    inGame = false;
                    stopwatch.Stop();
                    timer.Stop();
                    MessageBox.Show("They won in " + remoteGame.MovesTaken.ToString() + " moves in " + moves.Item3);
                    stopwatch.Reset();
                    localGame = new Game(poleCount, discCount, 0, poleCount - 1);
                    remoteGame = new Game(poleCount, discCount, 0, poleCount - 1);
                    scene.Reset(discCount, poleCount, 0, discHeight);
                }
            }
        }

        private void MoveDisc((int, int) move)
        {
            // see if its a valid, move play it if yes
            if (localGame.MoveDisc(move.Item1, move.Item2))
            {
                // valid move, move disc
                scene.DropDisc(localGame.PeekPole(move.Item2), move.Item2, localGame.NumberOnPole(move.Item2) - 1);
                string time = TimerOutput.Text;
                if (localGame.GameWon)
                {
                    inGame = false;
                    stopwatch.Stop();
                    timer.Stop();
                    UpdateTimerText(null, new EventArgs());
                    time = TimerOutput.Text;
                    TCP.SendMove(move.Item1, move.Item2, time);
                    MessageBox.Show("You won in " + localGame.MovesTaken.ToString() + " moves in " + time);
                    stopwatch.Reset();
                    localGame = new Game(poleCount, discCount, 0, poleCount - 1);
                    remoteGame = new Game(poleCount, discCount, 0, poleCount - 1);
                    scene.Reset(discCount, poleCount, 0, discHeight);
                }
                else
                {
                    TCP.SendMove(move.Item1, move.Item2, time);
                }
            }
            else if (localGame.NumberOnPole(move.Item1) != 0)
            {
                // invalid move, move the disc back to where it was
                scene.DropDisc(localGame.PeekPole(move.Item1), move.Item1, localGame.NumberOnPole(move.Item1) - 1);
            }
        }

        public void NewMultiplayer(int DiscCount, int PoleCount)
        {
            discCount = DiscCount;
            poleCount = PoleCount;
            scene.Reset(discCount, poleCount, 0, discHeight);
            localGame = new Game(poleCount, discCount, 0, poleCount - 1);
            remoteGame = new Game(poleCount, discCount, 0, poleCount - 1);
            inGame = false;
            Viewport.Focus();
        }

        public void Recolour()
        {
            scene.Recolour();
        }

        private void UpdateTimerText(object? sender, EventArgs e)
        {
            TimerOutput.Text = ((int)(stopwatch.Elapsed.TotalMinutes)).ToString("00") + ":" + (stopwatch.Elapsed.TotalSeconds % 60).ToString("00.000");
        }
    }
}
