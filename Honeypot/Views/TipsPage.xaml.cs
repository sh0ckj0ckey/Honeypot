using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Honeypot.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TipsPage : Page
    {
        public TipsPage()
        {
            this.InitializeComponent();
        }

        private async void OnClickDbFile(object sender, RoutedEventArgs e)
        {
            try
            {
                string folderPath = UserDataPaths.GetDefault().Documents;
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
                var dbNoMewingFolder = await folder.CreateFolderAsync("NoMewing", CreationCollisionOption.OpenIfExists);
                var dbFolder = await dbNoMewingFolder.CreateFolderAsync("Honeypot", CreationCollisionOption.OpenIfExists);
                await Launcher.LaunchFolderAsync(dbFolder);
            }
            catch { }
        }
    }
}
