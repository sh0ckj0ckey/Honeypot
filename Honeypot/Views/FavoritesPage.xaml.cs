using System;
using System.Diagnostics;
using Honeypot.Controls;
using Honeypot.Models;
using Honeypot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Honeypot.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FavoritesPage : Page
    {
        private MainViewModel MainViewModel = null;

        private ContentDialog _favoriteDetailDialog = null;

        public FavoritesPage()
        {
            this.InitializeComponent();

            MainViewModel = MainViewModel.Instance;

            _favoriteDetailDialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Content = new PasswordDetailControl(),
                Padding = new Thickness(0, 0, 0, 0),
                PrimaryButtonText = "取消收藏",
                CloseButtonText = "关闭",
                DefaultButton = ContentDialogButton.Close
            };
        }

        /// <summary>
        /// 点击查看收藏的密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickFavoritePassword(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.DataContext is PasswordModel passwordModel)
                {
                    MainViewModel.Instance.SelectedFavoritePassword = passwordModel;
                    _favoriteDetailDialog.XamlRoot = this.XamlRoot;
                    _favoriteDetailDialog.RequestedTheme = this.ActualTheme;
                    ContentDialogResult result = await _favoriteDetailDialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        MainViewModel.Instance.FavoritePassword(passwordModel);
                    }

                    MainViewModel.Instance.SelectedFavoritePassword = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn)
                {
                    Storyboard sb = btn.Resources["PointerEnterStoryboard"] as Storyboard;
                    sb.Begin();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn)
                {
                    Storyboard sb = btn.Resources["PointerLeaveStoryboard"] as Storyboard;
                    sb.Begin();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
