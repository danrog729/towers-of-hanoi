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

        BackgroundWorker[] animationThreads;
        AnimationData[] animationData;

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

            animationThreads = new BackgroundWorker[discCount];
            for (int threadIndex = 0; threadIndex < animationThreads.Length; threadIndex++)
            {
                animationThreads[threadIndex] = new BackgroundWorker();
                animationThreads[threadIndex].DoWork += AnimationTimer;
                animationThreads[threadIndex].WorkerReportsProgress = true;
                animationThreads[threadIndex].ProgressChanged += PerformAnimation;
                animationThreads[threadIndex].WorkerSupportsCancellation = true;
            }
            animationData = new AnimationData[discCount];

            pause = new ManualResetEventSlim(true);
            paused = true;
            readyToUnpause = true;
            delay = 500;
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
            CancelBotPlay();
            animationThreads = new BackgroundWorker[discCount];
            for (int threadIndex = 0; threadIndex < animationThreads.Length; threadIndex++)
            {
                animationThreads[threadIndex] = new BackgroundWorker();
                animationThreads[threadIndex].DoWork += AnimationTimer;
                animationThreads[threadIndex].WorkerReportsProgress = true;
                animationThreads[threadIndex].ProgressChanged += PerformAnimation;
                animationThreads[threadIndex].WorkerSupportsCancellation = true;
            }
            animationData = new AnimationData[discCount];
            game = new Game(poleCount, discCount, 0, poleCount - 1);
            pause = new ManualResetEventSlim(true);
            paused = true;
            readyToUnpause = true;
            Viewport.Focus();
        }

        public void CancelBotPlay()
        {
            // stop all animation threads
            for (int threadIndex = 0; threadIndex < animationThreads.Length; threadIndex++)
            {
                animationThreads[threadIndex].CancelAsync();
            }
        }

        private void PlayGame(object? sender, DoWorkEventArgs e)
        {
            bool firstIteration = true;
            while (!game.GameWon)
            {
                if (!firstIteration)
                {
                    Thread.Sleep(delay);
                }
                firstIteration = false;
                (int, int) move = game.BotMove();
                pause.Reset();
                botThread.ReportProgress(0, move);
                pause.Wait();
            }
        }

        private void UpdateViewport(object? sender, ProgressChangedEventArgs e)
        {
            readyToUnpause = false;
            (int, int)? nullableMove = e.UserState as (int,int)?;
            if (nullableMove != null)
            {
                (int, int) move = nullableMove.Value;
                int disc = game.PeekPole(move.Item2);
                int starting = move.Item1;
                int target = move.Item2;
                int numberOnTarget = game.NumberOnPole(move.Item2) - 1;
                (int, int, int, int) data = (disc, starting, target, numberOnTarget);
                AnimationSequence(data);
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

        private void AnimationSequence((int,int,int,int) move)
        {
            BackgroundWorker worker = animationThreads[move.Item1 - 1];
            animationData[move.Item1 - 1].Disc = move.Item1;
            animationData[move.Item1 - 1].StartingPole = move.Item2;
            animationData[move.Item1 - 1].TargetPole = move.Item3;
            animationData[move.Item1 - 1].NumberOnPole = move.Item4;
            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync(move.Item1 - 1);
            }
        }

        private void AnimationTimer(object? sender, DoWorkEventArgs e)
        {
            int? nullableData = e.Argument as int?;
            if (nullableData != null)
            {
                int index = nullableData.Value;
                BackgroundWorker? worker = sender as BackgroundWorker;
                if (worker != null)
                {
                    worker.ReportProgress(0, index);
                    Thread.Sleep((int)(Scene3D.hoverTime * 1000 / Preferences.AnimationSpeed));
                    if (e.Cancel)
                    {
                        return;
                    }
                    worker.ReportProgress(1, index);
                    Thread.Sleep((int)(Scene3D.hoverTime * 1000 / Preferences.AnimationSpeed));
                    if (e.Cancel)
                    {
                        return;
                    }
                    worker.ReportProgress(2, index);
                }
            }
        }

        private void PerformAnimation(object? sender, ProgressChangedEventArgs e)
        {
            int? nullableData = e.UserState as int?;
            if (nullableData != null)
            {
                int index = nullableData.Value;
                switch (e.ProgressPercentage)
                {
                    case 0:
                        scene.HoverDisc(animationData[index].Disc, animationData[index].StartingPole); break;
                    case 1:
                        scene.HoverDisc(animationData[index].Disc, animationData[index].TargetPole); break;
                    case 2:
                        scene.DropDisc(animationData[index].Disc, animationData[index].TargetPole, animationData[index].NumberOnPole); break;
                }
            }
        }

        private void DisplayWin(object? sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Bot won in " + game.MovesTaken.ToString() + " moves.");
            game = new Game(poleCount, discCount, 0, poleCount - 1);
            scene.Reset(discCount, poleCount, 0, discHeight);
            paused = true;
            readyToUnpause = true;
        }

        private void NextClicked(object sender, RoutedEventArgs e)
        {
            if (paused)
            {
                if (!game.GameWon)
                {
                    (int, int) move = game.BotMove();
                    int disc = game.PeekPole(move.Item2);
                    int starting = move.Item1;
                    int target = move.Item2;
                    int numberOnTarget = game.NumberOnPole(move.Item2) - 1;
                    (int, int, int, int) data = (disc, starting, target, numberOnTarget);
                    AnimationSequence(data);
                }
                if (game.GameWon)
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
                if (!botThread.IsBusy)
                {
                    botThread.RunWorkerAsync();
                }
            }
        }

        private void PreviousClicked(object sender, RoutedEventArgs e)
        {
            if (game.moveHistory.Count > 0 && paused)
            {
                (int, int) move = game.UndoMove();
                int disc = game.PeekPole(move.Item1);
                int starting = move.Item2;
                int target = move.Item1;
                int numberOnTarget = game.NumberOnPole(move.Item1) - 1;
                (int, int, int, int) data = (disc, starting, target, numberOnTarget);
                AnimationSequence(data);
            }
        }

        private void SpeedChanged(object sender, RoutedEventArgs e)
        {
            delay = (int)(1000 - SpeedSlider.Value);
        }
    }

    public struct AnimationData
    {
        public int Disc;
        public int StartingPole;
        public int TargetPole;
        public int NumberOnPole;
    }
}
