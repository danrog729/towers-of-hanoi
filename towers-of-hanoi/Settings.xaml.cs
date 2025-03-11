using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents.DocumentStructures;

namespace towers_of_hanoi
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        public static int DesiredWidth = 450;
        public static int DesiredHeight = 600;

        public Settings()
        {
            InitializeComponent();
            foreach (Theme theme in App.MainApp.themes)
            {
                ThemeSelector.Items.Add(new ComboBoxItem() { Content = theme.Name });
            }
            ThemeSelector.SelectedIndex = App.MainApp.themes.IndexOf(App.MainApp.CurrentTheme);
        }

        private void BackClicked(object sender, RoutedEventArgs e)
        {
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToMainMenu();
        }

        private void ChangeAnimationSpeed(object sender, EventArgs e)
        {
            Preferences.AnimationSpeed = (float)(AnimationSpeedSlider.Value);
        }

        public void ChangeTheme(object sender, RoutedEventArgs e)
        {
            App.MainApp.CurrentTheme = App.MainApp.themes[ThemeSelector.SelectedIndex];
        }
    }
}
