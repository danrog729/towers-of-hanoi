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

namespace towers_of_hanoi
{
    /// <summary>
    /// Interaction logic for Singleplayer.xaml
    /// </summary>
    public partial class Singleplayer : Page
    {
        Scene3D scene;
        Game game;
        Point lastMousePos;
        bool rightMouseDownLast;

        public int discCount = 6;
        public int poleCount = 3;
        float discHeight = 2;

        bool inGame;
        DispatcherTimer timer;
        Stopwatch stopwatch = new Stopwatch();

        public Singleplayer()
        {
            InitializeComponent();
            scene = new Scene3D(Viewport);
            game = new Game(3, discCount, 0, 2);
            scene.Reset(discCount, 3, 0, discHeight);
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
                    if (overPole != -1 && overPole != scene.DraggingLastOver && game.NumberOnPole(scene.DraggingFrom) != 0)
                    {
                        // hovering over a pole
                        scene.HoverDisc(game.PeekPole(scene.DraggingFrom), overPole);
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
            if (scene.ValidDragDrop && game.NumberOnPole(scene.DraggingFrom) != 0)
            {
                scene.HoverDisc(game.PeekPole(scene.DraggingFrom), scene.DraggingFrom);
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
                        if (game.NumberOnPole(poleNumber) != 0)
                        {
                            scene.SelectDirectMove(poleNumber);
                            scene.HoverDisc(game.PeekPole(scene.DraggingFrom), scene.DraggingFrom);
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

        private void MoveDisc((int, int) move)
        {
            // see if its a valid, move play it if yes
            if (game.MoveDisc(move.Item1, move.Item2))
            {
                // valid move, move disc
                scene.DropDisc(game.PeekPole(move.Item2), move.Item2, game.NumberOnPole(move.Item2) - 1);
                if (game.GameWon)
                {
                    inGame = false;
                    stopwatch.Stop();
                    timer.Stop();
                    MessageBox.Show("You won in " + game.MovesTaken.ToString() + " moves in " +
                        ((int)(stopwatch.Elapsed.TotalMinutes)).ToString("00") + ":" + (stopwatch.Elapsed.TotalSeconds % 60).ToString("00.000"));
                    stopwatch.Reset();
                    game = new Game(poleCount, discCount, 0, poleCount - 1);
                    scene.Reset(discCount, poleCount, 0, discHeight);
                }
            }
            else if (game.NumberOnPole(move.Item1) != 0)
            {
                // invalid move, move the disc back to where it was
                scene.DropDisc(game.PeekPole(move.Item1), move.Item1, game.NumberOnPole(move.Item1) - 1);
            }
        }

        public void NewSingleplayer(int DiscCount, int PoleCount)
        {
            discCount = DiscCount;
            poleCount = PoleCount;

            timer.Stop();
            stopwatch.Reset();
            TimerOutput.Text = "00:00.000";
            inGame = false;

            scene.Reset(discCount, poleCount, 0, discHeight);
            game = new Game(poleCount, discCount, 0, poleCount - 1);
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
