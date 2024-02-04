using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Honeypot.Controls;
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

        private PasswordEditingControl _editingPasswordControl = null;
        private ContentDialog _editPasswordDialog = null;

        public PasswordsPage()
        {
            this.InitializeComponent();

            MainViewModel = MainViewModel.Instance;

            MainViewModel.Instance.ActNoticeUserToBackup = CheckToNoticeUserBackup;

            _editingPasswordControl = new PasswordEditingControl();

            _editPasswordDialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Content = _editingPasswordControl,
                Padding = new Thickness(0, 0, 0, 0),
                PrimaryButtonText = "保存",
                CloseButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary
            };
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
        /// 点击编辑密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickEdit(object sender, RoutedEventArgs e)
        {
            try
            {
                var editing = MainViewModel.Instance.SelectedPassword;


                //_editPasswordDialog.IsPrimaryButtonEnabled = false;


                _editingPasswordControl.SetOriginInfo(
                    editing.Id,
                    editing.Account,
                    editing.Password,
                    editing.Name,
                    editing.Website,
                    editing.Note,
                    editing.CategoryId,
                    editing.LogoFileName);

                _editPasswordDialog.XamlRoot = this.XamlRoot;
                _editPasswordDialog.RequestedTheme = this.ActualTheme;
                ContentDialogResult result = await _editPasswordDialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    int id = _editingPasswordControl.GetModifiedInfo(
                                 out string account,
                                 out string password,
                                 out string name,
                                 out string website,
                                 out string note,
                                 out int categoryId,
                                 out string logoFile);

                    if (id == editing.Id)
                    {
                        MainViewModel.Instance.EditPassword(editing, categoryId, account, password, name, website, note, editing.Favorite, logoFile);
                    }
                }

                _editingPasswordControl.ResetView();
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

        /// <summary>
        /// 点击了解更多关于备份数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickGetInfoAboutBackup(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.ActNavigatePage?.Invoke(NavigatePageEnum.Tips);
        }

        /// <summary>
        /// 如果时机合适，则提示用户记得备份
        /// </summary>
        /// <param name="count"></param>
        private void CheckToNoticeUserBackup(int count)
        {
            try
            {
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (count == 1 && localSettings.Values["countReach1"] == null)
                {
                    localSettings.Values["countReach1"] = true;
                }
                else if (count == 10 && localSettings.Values["countReach10"] == null)
                {
                    localSettings.Values["countReach10"] = true;
                }
                else if (count == 20 && localSettings.Values["countReach20"] == null)
                {
                    localSettings.Values["countReach20"] = true;
                }
                else if (count == 50 && localSettings.Values["countReach50"] == null)
                {
                    localSettings.Values["countReach50"] = true;
                }
                else if (count == 100 && localSettings.Values["countReach100"] == null)
                {
                    localSettings.Values["countReach100"] = true;
                }
                else if (count == 200 && localSettings.Values["countReach200"] == null)
                {
                    localSettings.Values["countReach200"] = true;
                }
                else if (count == 300 && localSettings.Values["countReach300"] == null)
                {
                    localSettings.Values["countReach300"] = true;
                }
                else if (count == 500 && localSettings.Values["countReach500"] == null)
                {
                    localSettings.Values["countReach500"] = true;
                }
                else
                {
                    return;
                }

                string message = "哇，你已经记录了这么多个账号，真不错，但是要记得定期备份一下数据哦，不要让数据面临丢失的风险。";
                if (count == 1)
                {
                    message = "恭喜！你刚刚记录了第一个账号，真不错，要记得定期备份一下数据哦，不要让数据面临丢失的风险。";
                }
                else if (count > 1)
                {
                    message = $"哇，你已经记录了 {count} 个账号，真不错，但是要记得定期备份一下数据哦，不要让数据面临丢失的风险。";
                }
                RememberBackupInfoBar.Message = message;
                RememberBackupInfoBar.IsOpen = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
