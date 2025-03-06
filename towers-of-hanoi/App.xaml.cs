using System.Configuration;
using System.Data;
using System.Windows;

namespace towers_of_hanoi
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static App MainApp => ((App)Current);

        public static Preferences preferences = new Preferences();
    }

    public class Preferences
    {
        public float animationSpeed = 1;

    }
}
