using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
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

namespace towers_of_hanoi
{
    /// <summary>
    /// Interaction logic for Automatic.xaml
    /// </summary>
    public partial class Automatic : Page
    {
        Scene3D scene;
        Game game;
        Point lastMousePos;
        bool rightMouseDownLast;

        public int discCount = 6;
        public int poleCount = 3;
        float discHeight = 2;

        BackgroundWorker botThread;
        ManualResetEventSlim pause;
        bool paused;
        bool readyToUnpause;
        int delay;

        public Automatic()
        {
            InitializeComponent();
            scene = new Scene3D(Viewport);
            game = new Game(3, discCount, 0, 2);
            scene.Reset(discCount, 3, 0, discHeight);
            lastMousePos = new Point(0, 0);
            rightMouseDownLast = false;

            botThread = new BackgroundWorker();
            botThread.DoWork += PlayGame;
            botThread.WorkerReportsProgress = true;
            botThread.ProgressChanged += UpdateViewport;
            botThread.RunWorkerCompleted += DisplayWin;

            pause = new ManualResetEventSlim(true);
            paused = false;
            readyToUnpause = false;
            delay = 1000;

            App.MainApp.animationSpeed = 1.0f;
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
        }

        private void ViewportMouseScrolled(object sender, MouseWheelEventArgs e)
        {
            scene.ZoomCamera(e.Delta * 0.0005f);
        }

        public void NewAutomatic(int DiscCount, int PoleCount)
        {
            discCount = DiscCount;
            poleCount = PoleCount;
            scene.Reset(discCount, poleCount, 0, discHeight);
            game = new Game(poleCount, discCount, 0, poleCount - 1);
            StartButton.Visibility = Visibility.Visible;
            Viewport.Focus();
        }

        private void PlayGame(object? sender, DoWorkEventArgs e)
        {
            while (!game.GameWon)
            {
                (int, int) move = game.BotMove();
                pause.Reset();
                botThread.ReportProgress(0, move);
                pause.Wait();
                Thread.Sleep(delay);
            }
        }

        private void UpdateViewport(object? sender, ProgressChangedEventArgs e)
        {
            readyToUnpause = false;
            (int, int)? nullableMove = e.UserState as (int,int)?;
            if (nullableMove != null)
            {
                (int, int) move = nullableMove.Value;
                scene.HoverDisc(game.PeekPole(move.Item2), move.Item1);
                scene.HoverDisc(game.PeekPole(move.Item2), move.Item2);
                scene.DropDisc(game.PeekPole(move.Item2), move.Item2, game.NumberOnPole(move.Item2) - 1);
            }
            if (!paused)
            {
                pause.Set();
            }
            else
            {
                readyToUnpause = true;
            }
        }

        private void StartGame(object sender, RoutedEventArgs e)
        {
            botThread.RunWorkerAsync();
            StartButton.Visibility = Visibility.Hidden;
        }

        private void DisplayWin(object? sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Bot won in " + game.MovesTaken.ToString() + " moves.");
            game = new Game(poleCount, discCount, 0, poleCount - 1);
            scene.Reset(discCount, poleCount, 0, discHeight);
            StartButton.Visibility = Visibility.Visible;
        }

        private void NextClicked(object sender, RoutedEventArgs e)
        {
            if (paused)
            {
                if (!game.GameWon)
                {
                    (int, int) move = game.BotMove();
                    scene.DropDisc(game.PeekPole(move.Item2), move.Item2, game.NumberOnPole(move.Item2) - 1);
                }
                else
                {
                    DisplayWin(null, new RunWorkerCompletedEventArgs(null, null, false));
                }
            }
        }

        private void PauseClicked(object sender, RoutedEventArgs e)
        {
            paused = !paused;
            if (!paused && readyToUnpause)
            {
                pause.Set();
            }
        }

        private void PreviousClicked(object sender, RoutedEventArgs e)
        {
            if (game.moveHistory.Count > 0 && paused)
            {
                (int, int) move = game.UndoMove();
                scene.DropDisc(game.PeekPole(move.Item1), move.Item1, game.NumberOnPole(move.Item1) - 1);
            }
        }
    }
}
