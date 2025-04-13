using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Honeypot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Honeypot.Controls
{
    public sealed partial class PasswordDetailControl : UserControl
    {
        private MainViewModel MainViewModel = null;

        public PasswordDetailControl()
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
            if (!string.IsNullOrWhiteSpace(MainViewModel.Instance.SelectedFavoritePassword.Account))
            {
                DataPackage dataPackage = new();
                dataPackage.SetText(MainViewModel.Instance.SelectedFavoritePassword.Account);
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
            if (!string.IsNullOrWhiteSpace(MainViewModel.Instance.SelectedFavoritePassword.Password))
            {
                DataPackage dataPackage = new();
                dataPackage.SetText(MainViewModel.Instance.SelectedFavoritePassword.Password);
                Clipboard.SetContent(dataPackage);

                PasswordButtonTextBlock.Opacity = 0;
                PasswordCopiedFontIcon.Visibility = Visibility.Visible;
                await Task.Delay(800);
                PasswordButtonTextBlock.Opacity = 1;
                PasswordCopiedFontIcon.Visibility = Visibility.Collapsed;
            }
        }

        [GeneratedRegex("^https?:\\/\\/", RegexOptions.IgnoreCase)]
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
                string url = MainViewModel.Instance.SelectedFavoritePassword.Website.Trim();
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

    }
}
