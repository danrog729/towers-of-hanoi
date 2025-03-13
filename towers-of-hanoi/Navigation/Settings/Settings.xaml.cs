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

        bool settingChanged;

        public Settings()
        {
            InitializeComponent();
            foreach (Theme theme in App.MainApp.themes)
            {
                ThemeSelector.Items.Add(new ComboBoxItem() { Content = theme.Name });
            }
            ThemeSelector.SelectedIndex = App.MainApp.themes.IndexOf(App.MainApp.CurrentTheme);
            settingChanged = false;
        }

        private void BackClicked(object sender, RoutedEventArgs e)
        {
            App.MainApp.clickSound.Play();
            // confirm exit if settings have been changed
            if (settingChanged)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to exit without saving?", "Exit Settings", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    // reset all of the settings
                    AnimationSpeedSlider.Value = Preferences.AnimationSpeed;
                    ThemeSelector.SelectedIndex = App.MainApp.themes.IndexOf(App.MainApp.CurrentTheme);
                    SoundsCheckbox.IsChecked = Preferences.SoundsOn;

                    // return to main menu
                    settingChanged = false;
                    ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToMainMenu();
                }
            }
            else
            {
                ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToMainMenu();
            }
        }

        private void ConfirmClicked(object sender, RoutedEventArgs e)
        {
            App.MainApp.clickSound.Play();
            // set all of the settings
            Preferences.AnimationSpeed = (float)(AnimationSpeedSlider.Value);
            App.MainApp.CurrentTheme = App.MainApp.themes[ThemeSelector.SelectedIndex];
            if (SoundsCheckbox.IsChecked != null)
            {
                Preferences.SoundsOn = SoundsCheckbox.IsChecked.Value;
            }
            settingChanged = false;

            // return to main menu
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToMainMenu();
        }

        private void SettingChanged(object sender, EventArgs e)
        {
            settingChanged = true;
        }
    }
}
