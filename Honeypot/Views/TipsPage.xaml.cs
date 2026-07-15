using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Honeypot.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class TipsPage : Page
{
    public TipsPage()
    {
        this.InitializeComponent();
    }

    private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string documentsFolderPath = UserDataPaths.GetDefault().Documents;
            StorageFolder documentsFolder = await StorageFolder.GetFolderFromPathAsync(documentsFolderPath);
            StorageFolder noMewingFolder = await documentsFolder.CreateFolderAsync("NoMewing", CreationCollisionOption.OpenIfExists);
            StorageFolder honeypotFolder = await noMewingFolder.CreateFolderAsync("Honeypot", CreationCollisionOption.OpenIfExists);
            await Launcher.LaunchFolderAsync(honeypotFolder);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine(ex);
        }
    }
}
