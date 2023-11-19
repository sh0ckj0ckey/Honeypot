using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ManyPasswords3.Core.Utils;
using ManyPasswords3.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ManyPasswords3.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private MainViewModel _viewModel = null;
        private string _appVersion = string.Empty;

        public SettingsPage()
        {
            this.InitializeComponent();

            _viewModel = MainViewModel.Instance;
            _appVersion = $"v{AppVersionUtil.GetAppVersion()}";
        }

        /// <summary>
        /// 打分评价
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickGoToStoreRate(object sender, RoutedEventArgs e)
        {
            try
            {
                await Launcher.LaunchUriAsync(new Uri($"ms-windows-store:REVIEW?PFN={Package.Current.Id.FamilyName}"));
            }
            catch { }
        }

        /// <summary>
        /// 查看数据库目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickDbPath(object sender, RoutedEventArgs e)
        {
            try
            {
                string folderPath = UserDataPaths.GetDefault().Documents;
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
                var dbNoMewingFolder = await folder.CreateFolderAsync("NoMewing", CreationCollisionOption.OpenIfExists);
                var dbFolder = await dbNoMewingFolder.CreateFolderAsync("ManyPasswords", CreationCollisionOption.OpenIfExists);
                await Launcher.LaunchFolderAsync(dbFolder);
            }
            catch { }
        }

        /// <summary>
        /// 访问 GitHub
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickGoGitHub(object sender, RoutedEventArgs e)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/sh0ckj0ckey/ManyPasswords"));
            }
            catch { }
        }

        /// <summary>
        /// 查看 Windows Hello 的设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickGoWindowsHelloSettings(object sender, RoutedEventArgs e)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:signinoptions"));
            }
            catch { }
        }

        /// <summary>
        /// 切换 Windows Hello 锁定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnToggleWindowHello(object sender, RoutedEventArgs e)
        {

        }
    }
}
