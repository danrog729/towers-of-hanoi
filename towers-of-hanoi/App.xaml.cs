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
        public List<Theme> themes = new List<Theme>();
        private Theme _currentTheme;

        public Theme CurrentTheme
        {
            get => _currentTheme;
            set
            {
                _currentTheme = value;
                ResourceDictionary styles = new ResourceDictionary() { Source = new Uri("Navigation/Settings/Themes/Styles.xaml", UriKind.Relative) };
                ResourceDictionary theme = new ResourceDictionary() { Source = new Uri(_currentTheme.Path, UriKind.Relative) };
                theme.MergedDictionaries.Add(styles);
                Resources.MergedDictionaries.Clear();
                Resources.MergedDictionaries.Add(theme);

                // recolour discs
                if (MainApp.MainWindow != null)
                {
                    ((MainWindow)(MainApp.MainWindow)).singleplayer.Recolour();
                    ((MainWindow)(MainApp.MainWindow)).navigationWindow.Recolour();
                    ((MainWindow)(MainApp.MainWindow)).automatic.Recolour();
                }
            }
        }

        public static App MainApp => ((App)Current);

        public App()
        {
            InitializeComponent();

            themes.Add(new Theme("Light", "Navigation/Settings/Themes/Light.xaml"));
            themes.Add(new Theme("Dark", "Navigation/Settings/Themes/Dark.xaml"));
            themes.Add(new Theme("High Contrast Light", "Navigation/Settings/Themes/HighContrastLight.xaml"));
            themes.Add(new Theme("High Contrast Dark", "Navigation/Settings/Themes/HighContrastDark.xaml"));
            themes.Add(new Theme("Colourful", "Navigation/Settings/Themes/Colourful.xaml"));
            themes.Add(new Theme("Industrial", "Navigation/Settings/Themes/Factorio.xaml"));
            themes.Add(new Theme("Space Age", "Navigation/Settings/Themes/Kerbal.xaml"));
            themes.Add(new Theme("Programmer", "Navigation/Settings/Themes/Perry.xaml"));
            _currentTheme = themes[0];
            CurrentTheme = themes[0];
        }
    }

    public class Theme
    {
        public string Name;
        public string Path;
        public Theme(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }

    public static class Preferences
    {
        public static float AnimationSpeed = 1.0f;
    }
}
