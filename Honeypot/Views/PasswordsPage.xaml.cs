using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Honeypot.Models;
using Honeypot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;

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

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SearchAutoSuggestBox.Text = "";
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

        /// <summary>
        /// 输入文本搜索密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnSearchTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            try
            {
                var suggests = MainViewModel.Instance.SearchPasswords(sender.Text.Trim());
                sender.ItemsSource = suggests;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 选中密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnChosenSuggestion(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            try
            {
                if (args.SelectedItem is PasswordModel password)
                {
                    if (password.Name.Contains(sender.Text, StringComparison.CurrentCultureIgnoreCase) || password.Name == sender.Text)
                    {
                        sender.Text = password.Name;
                    }
                    else
                    {
                        sender.Text = password.Account;
                    }

                    MainViewModel.Instance.SelectedPassword = password;

                    GroupedPasswordsList.ScrollIntoView(password);
                    TimeOrderPasswordsList.ScrollIntoView(password);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
