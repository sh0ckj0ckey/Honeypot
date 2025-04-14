using System;
using System.Diagnostics;
using Honeypot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Honeypot.Views
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
            _appVersion = $"{GetAppVersion()}";
        }

        /// <summary>
        /// 获取应用程序的版本号
        /// </summary>
        /// <returns></returns>
        private string GetAppVersion()
        {
            try
            {
                Package package = Package.Current;
                PackageId packageId = package.Id;
                PackageVersion version = packageId.Version;
                return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            }
            catch (Exception) { }
            return "";
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
                var dbFolder = await dbNoMewingFolder.CreateFolderAsync("Honeypot", CreationCollisionOption.OpenIfExists);
                await Launcher.LaunchFolderAsync(dbFolder);
            }
            catch { }
        }

        /// <summary>
        /// 点击开始迁移数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickMigrate(object sender, RoutedEventArgs e)
        {
            try
            {
                MainViewModel.Instance.ActInvokeMigrater?.Invoke(true);
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
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/sh0ckj0ckey/Honeypot"));
            }
            catch { }
        }

        /// <summary>
        /// 访问朱雀 GitHub
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickGoZhuque(object sender, RoutedEventArgs e)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/TrionesType/zhuque"));
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
            SetWindowsHelloEnable(WindowsHelloToggleSwitch.IsOn);
        }

        /// <summary>
        /// 开关WindowsHello
        /// </summary>
        /// <param name="on"></param>
        private async void SetWindowsHelloEnable(bool on)
        {
            try
            {
                if (MainViewModel.Instance.AppSettings.EnableLock != on)
                {
                    Debug.WriteLine("Verifying Windows Hello...");

                    var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();

                    switch (await Windows.Security.Credentials.UI.UserConsentVerifier.RequestVerificationAsync(resourceLoader.GetString("UnlockAppUnlockingMessage")))
                    {
                        case Windows.Security.Credentials.UI.UserConsentVerificationResult.Verified:
                            MainViewModel.Instance.AppSettings.EnableLock = on;
                            break;
                        case Windows.Security.Credentials.UI.UserConsentVerificationResult.DeviceNotPresent:
                        case Windows.Security.Credentials.UI.UserConsentVerificationResult.NotConfiguredForUser:
                        case Windows.Security.Credentials.UI.UserConsentVerificationResult.DisabledByPolicy:
                            WindowsHelloToggleSwitch.IsOn = MainViewModel.Instance.AppSettings.EnableLock;
                            MainViewModel.Instance.ShowTipsContentDialog(resourceLoader.GetString("UnlockAppUnlockFailed"), resourceLoader.GetString("UnlockAppDeviceUnavailable"));
                            break;
                        case Windows.Security.Credentials.UI.UserConsentVerificationResult.DeviceBusy:
                            WindowsHelloToggleSwitch.IsOn = MainViewModel.Instance.AppSettings.EnableLock;
                            MainViewModel.Instance.ShowTipsContentDialog(resourceLoader.GetString("UnlockAppUnlockFailed"), resourceLoader.GetString("UnlockAppDeviceBusy"));
                            break;
                        case Windows.Security.Credentials.UI.UserConsentVerificationResult.RetriesExhausted:
                            WindowsHelloToggleSwitch.IsOn = MainViewModel.Instance.AppSettings.EnableLock;
                            MainViewModel.Instance.ShowTipsContentDialog(resourceLoader.GetString("UnlockAppUnlockFailed"), resourceLoader.GetString("UnlockAppRetriesExhausted"));
                            break;
                        default:
                            WindowsHelloToggleSwitch.IsOn = MainViewModel.Instance.AppSettings.EnableLock;
                            break;
                    }
                }
            }
            catch { }
        }
    }
}
