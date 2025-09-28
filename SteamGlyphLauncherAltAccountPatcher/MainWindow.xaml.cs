using System;
using System.Collections.Generic;
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

namespace SteamGlyphLauncherAltAccountPatcher
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public GlyphClientApp GlyphClientApp { get; } = new GlyphClientApp();

        private void BrowseButton_Click( object sender, RoutedEventArgs e )
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select GlyphClientApp.exe",
                Filter = "Executable Files (*.exe)|*.exe"
            };

            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                GlyphClientApp.FilePath = openFileDialog.FileName;
            }
        }

        private void PatchButton_Click( object sender, RoutedEventArgs e )
        {
            if ( GlyphClientApp.IsPatched )
            {
                GlyphClientApp.Unpatch();
            }
            else
            {
                GlyphClientApp.Patch();
            }
        }
    }
}
