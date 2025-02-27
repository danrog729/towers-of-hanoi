using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.Diagnostics;

namespace towers_of_hanoi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Scene3D scene;
        Game game;
        Point lastMousePos;
        bool rightMouseDownLast;
        bool leftMouseDownLast;

        int discCount = 6;
        int poleCount = 3;
        float discHeight = 2;

        public MainWindow()
        {
            InitializeComponent();
            scene = new Scene3D(Viewport);
            game = new Game(3, discCount, 0, 2);
            scene.Reset(discCount, 3, 0, discHeight);
            lastMousePos = new Point(0, 0);
            rightMouseDownLast = false;
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
            System.Windows.Point currentPos = e.GetPosition(Viewport);
            scene.SelectObjectForDragAndDrop(currentPos);
            if (scene.ValidDragDrop && game.NumberOnPole(scene.DraggingFrom) != 0)
            {
                scene.HoverDisc(game.PeekPole(scene.DraggingFrom), scene.DraggingFrom);
                scene.DraggingLastOver = scene.DraggingFrom;
            }
        }

        private void ViewportLeftMouseUp(object sender, MouseEventArgs e)
        {
            // figure out the move thats just been played
            System.Windows.Point currentPos = e.GetPosition(Viewport);
            (int, int) move = scene.ReleaseDragAndDrop(currentPos);

            // see if its a valid, move play it if yes
            if (game.MoveDisc(move.Item1, move.Item2))
            {
                // valid move, move disc
                scene.DropDisc(game.PeekPole(move.Item2), move.Item2, game.NumberOnPole(move.Item2) - 1);
                if (game.GameWon)
                {
                    MessageBox.Show("You won in " + game.MovesTaken.ToString() + " moves!");
                    game = new Game(poleCount, discCount, 0, 2);
                    scene.Reset(discCount, poleCount, 0, discHeight);
                }
            }
            else if (game.NumberOnPole(move.Item1) != 0)
            {
                // invalid move, move the disc back to where it was
                scene.DropDisc(game.PeekPole(move.Item1), move.Item1, game.NumberOnPole(move.Item1) - 1);
            }
        }

        private void CountsChanged(object sender, EventArgs e)
        {
            if (Int32.TryParse(PoleCount.Text, out int pCount) && pCount >= 3 && pCount <= 10 && scene != null)
            {
                poleCount = pCount;
                if (Int32.TryParse(DiscCount.Text, out int count) && count <= 20)
                {
                    discCount = count;
                    game = new Game(poleCount, discCount, 0, poleCount - 1);
                    scene.Reset(discCount, poleCount, 0, discHeight);
                }
            }
        }
    }
}