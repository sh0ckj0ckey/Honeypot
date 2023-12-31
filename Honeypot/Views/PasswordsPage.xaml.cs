using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Honeypot.ViewModels;
using static System.Net.Mime.MediaTypeNames;
using Windows.ApplicationModel.DataTransfer;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.RegularExpressions;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Honeypot.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PasswordsPage : Page
    {
        private MainViewModel MainViewModel = null;

        public PasswordsPage()
        {
            this.InitializeComponent();

            MainViewModel = MainViewModel.Instance;
        }

        /// <summary>
        /// 点击复制账号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickCopyAccount(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(MainViewModel.Instance.SelectedPassword.Account))
            {
                DataPackage dataPackage = new();
                dataPackage.SetText(MainViewModel.Instance.SelectedPassword.Account);
                Clipboard.SetContent(dataPackage);

                AccountButtonTextBlock.Opacity = 0;
                AccountCopiedFontIcon.Visibility = Visibility.Visible;
                await Task.Delay(800);
                AccountButtonTextBlock.Opacity = 1;
                AccountCopiedFontIcon.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 点击复制密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickCopyPassword(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(MainViewModel.Instance.SelectedPassword.Password))
            {
                DataPackage dataPackage = new();
                dataPackage.SetText(MainViewModel.Instance.SelectedPassword.Password);
                Clipboard.SetContent(dataPackage);

                PasswordButtonTextBlock.Opacity = 0;
                PasswordCopiedFontIcon.Visibility = Visibility.Visible;
                await Task.Delay(800);
                PasswordButtonTextBlock.Opacity = 1;
                PasswordCopiedFontIcon.Visibility = Visibility.Collapsed;
            }
        }

        [GeneratedRegex("^https?:\\/\\/", RegexOptions.IgnoreCase, "zh-CN")]
        private static partial Regex _urlSchemeRegex();

        /// <summary>
        /// 点击跳转网址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickWebsite(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = MainViewModel.Instance.SelectedPassword.Website.Trim();
                if (!string.IsNullOrWhiteSpace(url))
                {
                    if (!_urlSchemeRegex().IsMatch(url))
                    {
                        url = "https://" + url;
                    }

                    if (Uri.TryCreate(url, UriKind.Absolute, out Uri createdUrl))
                    {
                        if (createdUrl.Scheme == Uri.UriSchemeHttp || createdUrl.Scheme == Uri.UriSchemeHttps)
                        {
                            await Windows.System.Launcher.LaunchUriAsync(createdUrl);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 点击确认删除密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickDelete(object sender, RoutedEventArgs e)
        {
            try
            {
                MainViewModel.Instance.DeletePassword(MainViewModel.Instance.SelectedPassword);
                DeletePasswordFlyout.Hide();

                MainViewModel.Instance.SelectedPassword = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 点击收藏/取消收藏密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickFavorite(object sender, RoutedEventArgs e)
        {
            try
            {
                MainViewModel.Instance.FavoritePassword(MainViewModel.Instance.SelectedPassword);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 点击关闭页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickClose(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.SelectedPassword = null;
        }
    }
}
