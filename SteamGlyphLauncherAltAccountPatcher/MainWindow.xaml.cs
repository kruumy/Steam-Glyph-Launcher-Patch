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

        private async void PatchButton_Click( object sender, RoutedEventArgs e )
        {
            PatchButton.IsEnabled = false;
            if ( GlyphClientApp.IsPatched )
            {
                GlyphClientApp.Unpatch();
            }
            else
            {
                GlyphClientApp.Patch();
            }
            await Task.Delay(1000);
            PatchButton.IsEnabled = true;
        }

        void UpdateStatusText()
        {
            switch ( GlyphClientApp.LastStatus )
            {
                case GlyphClientApp.Status.WaitingForFilePath:
                    StatusTextBlock.Text = "Waiting for GlyphClientApp.exe...";
                    break;
                case GlyphClientApp.Status.InvalidFilePath:
                    StatusTextBlock.Text = "Invalid file path. Please select a valid GlyphClientApp.exe...";
                    break;
                case GlyphClientApp.Status.ValidFilePathNotPatched:
                    StatusTextBlock.Text = "GlyphClientApp.exe is valid and not patched.";
                    break;
                case GlyphClientApp.Status.ValidFilePathPatched:
                    StatusTextBlock.Text = "GlyphClientApp.exe is valid and patched.";
                    break;
                case GlyphClientApp.Status.SuccessfullyPatched:
                    StatusTextBlock.Text = "Successfully Patched!";
                    break;
                case GlyphClientApp.Status.SuccessfullyUnpatched:
                    StatusTextBlock.Text = "Successfully Unpatched!";
                    break;
                case GlyphClientApp.Status.PatchFailed:
                    StatusTextBlock.Text = "Patch Failed :(";
                    break;
                case GlyphClientApp.Status.UnpatchFailed:
                    StatusTextBlock.Text = "Unpatch Failed :(";
                    break;
                default:
                    StatusTextBlock.Text = GlyphClientApp.LastStatus.ToString();
                    break;
            }

        }
        private void StatusTextBlock_Initialized( object sender, EventArgs e )
        {
            UpdateStatusText();
            GlyphClientApp.PropertyChanged += ( s, ev ) =>
            {
                if ( ev.PropertyName == nameof(GlyphClientApp.LastStatus) )
                {
                    this.UpdateStatusText();
                }
            };
        }
    }
}
