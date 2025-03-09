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
    }

    public static class Preferences
    {
        public static class Appearance
        {
            public static class Presets
            {

            }
            public static class Menus
            {

            }
            public static class Scene
            {
                public static class Background
                {

                }
                public static class Poles
                {

                }
                public static class Discs
                {

                }
            }
        }
        public static class Animation
        {
            public static float animationSpeed = 1.0f;
        }
        public static class Sounds
        {

        }
        public static class Controls
        {

        }
    }
}
