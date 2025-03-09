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

        public List<SettingsPage> pages = new List<SettingsPage>();

        public Settings()
        {
            InitializeComponent();

            CreateSettingsPages();
            SetUpTree();
        }

        private void BackClicked(object sender, RoutedEventArgs e)
        {
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToMainMenu();
        }

        public void CreateSettingsPages()
        {
            pages = new List<SettingsPage>();

            SettingsPage appearance = new SettingsPage("Appearance",
                [],
                [
                    new SettingsPage("Presets"),
                    new SettingsPage("Menus"),
                    new SettingsPage("3D Scene",
                    [],
                    [
                        new SettingsPage("Background"),
                        new SettingsPage("Poles",
                        [],
                        [
                            new SettingsPage("Pole 1"),
                            new SettingsPage("Pole 2"),
                            new SettingsPage("Pole 3"),
                            new SettingsPage("Pole 4"),
                            new SettingsPage("Pole 5"),
                            new SettingsPage("Pole 6"),
                            new SettingsPage("Pole 7"),
                            new SettingsPage("Pole 8"),
                            new SettingsPage("Pole 9"),
                            new SettingsPage("Pole 10"),
                            ]),
                        new SettingsPage("Discs",
                        [],
                        [
                            new SettingsPage("Disc 1"),
                            new SettingsPage("Disc 2"),
                            new SettingsPage("Disc 3"),
                            new SettingsPage("Disc 4"),
                            new SettingsPage("Disc 5"),
                            new SettingsPage("Disc 6"),
                            new SettingsPage("Disc 7"),
                            new SettingsPage("Disc 8"),
                            new SettingsPage("Disc 9"),
                            new SettingsPage("Disc 10"),
                            new SettingsPage("Disc 11"),
                            new SettingsPage("Disc 12"),
                            new SettingsPage("Disc 13"),
                            new SettingsPage("Disc 14"),
                            new SettingsPage("Disc 15"),
                            new SettingsPage("Disc 16"),
                            new SettingsPage("Disc 17"),
                            new SettingsPage("Disc 18"),
                            new SettingsPage("Disc 19"),
                            new SettingsPage("Disc 20"),
                            ]),
                        ])
                    ]);
            pages.Add(appearance);
            pages.Add(new SettingsPage("Animation",
                [new FloatSetting("Animation Speed", 0.01f, 1.0f, 10.0f)],
                []));
            pages.Add(new SettingsPage("Sounds"));
            pages.Add(new SettingsPage("Controls"));
        }

        public void ChangeSettingsPage(object sender, RoutedPropertyChangedEventArgs<Object> e)
        {
            TreeViewItem? treeViewItem = e.NewValue as TreeViewItem;
            if (treeViewItem != null)
            {
                SettingsPage? page = treeViewItem.Tag as SettingsPage;
                if (page != null)
                {
                    SettingsPageContent.Children.Clear();
                    foreach (Setting setting in page.settings)
                    {
                        if (setting is FloatSetting floatSetting)
                        {
                            SettingsPageContent.Children.Add(new SettingsControls.FloatSlider()
                            {
                                MinValue = floatSetting.Minimum,
                                Value = floatSetting.Value,
                                MaxValue = floatSetting.Maximum,
                                SettingLabel = floatSetting.Name
                            });
                        }
                    }
                }
            }
        }

        public void SetUpTree(TreeViewItem? branchItem = null, SettingsPage? branchPage = null)
        {
            if (branchItem == null || branchPage == null)
            {
                // start from the beginning
                SettingsTree.Items.Clear();
                foreach (SettingsPage page in pages)
                {
                    TreeViewItem treeViewItem = new TreeViewItem()
                    {
                        Header = page.Name,
                        Tag = page
                    };
                    SettingsTree.Items.Add(treeViewItem);
                    SetUpTree(treeViewItem, page);
                }
            }
            else
            {
                // add the children of this page
                foreach (SettingsPage page in branchPage.SubPages)
                {
                    TreeViewItem treeViewItem = new TreeViewItem()
                    {
                        Header = page.Name,
                        Tag = page
                    };
                    branchItem.Items.Add(treeViewItem);
                    SetUpTree(treeViewItem, page);
                }
            }
        }
    }

    public class SettingsPage
    {
        public string Name;
        public List<Setting> settings;
        public List<SettingsPage> SubPages;

        public SettingsPage(string name)
        {
            Name = name;
            settings = new List<Setting>();
            SubPages = new List<SettingsPage>();
        }

        public SettingsPage(string name, Setting[] settings)
        {
            Name = name;
            this.settings = settings.ToList();
            SubPages = new List<SettingsPage>();
        }

        public SettingsPage(string name, Setting[] settings, SettingsPage[] subPages)
        {
            Name = name;
            this.settings = settings.ToList();
            SubPages = subPages.ToList();
        }

        public void AddSetting(Setting setting)
        {
            if (!settings.Contains(setting))
            {
                settings.Add(setting);
            }
        }

        public void AddSubPage(SettingsPage subPage)
        {
            if (!SubPages.Contains(subPage))
            {
                SubPages.Add(subPage);
            }
        }
    }

    public class Setting
    {
        public string Name;
        public Setting(string name)
        {
            Name = name;
        }
    }

    public class FloatSetting : Setting
    {
        public float Minimum;
        public float Value;
        public float Maximum;
        public FloatSetting(string name, float minimum, float value, float maximum) : base(name)
        {
            Minimum = minimum;
            Value = value;
            Maximum = maximum;
        }
    }
}
