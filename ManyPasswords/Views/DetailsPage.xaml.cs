using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 好多密码_UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DetailsPage : Page
    {
        private bool IsAppBarVisible;
        public OnePassword ShowPassword = new OnePassword();
        public static DetailsPage Details = null;

        public DetailsPage()
        {
            this.InitializeComponent();
            Details = this;
            if (PasswordPage.Selected != null)
            {
                ShowPassword = PasswordPage.Selected;
            }
            if (App.AppSettingContainer.Values["Theme"] == null || App.AppSettingContainer.Values["Theme"].ToString() == "Light")
            {
                PhotoPanel.Opacity = 1;
            }
            else
            {
                PhotoPanel.Opacity = 0.7;
            }
            if (ShowPassword.IsFavorite == true)
            {
                FavoriteIcon.Visibility = Visibility.Collapsed;
                UnFavoriteIcon.Visibility = Visibility.Visible;
            }
            else
            {
                FavoriteIcon.Visibility = Visibility.Visible;
                UnFavoriteIcon.Visibility = Visibility.Collapsed;
            }
            IsAppBarVisible = true;
        }

        /// <summary>
        /// 隐藏操作栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsAppBarVisible == true)
            {
                AppBarPopOut.Begin();
                IsAppBarVisible = false;
            }
        }

        /// <summary>
        /// 点击空白区域显示操作栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (IsAppBarVisible == false)
            {
                AppBarPopIn.Begin();
                IsAppBarVisible = true;
            }
        }

        /// <summary>
        /// 复制账号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Windows.ApplicationModel.DataTransfer.DataPackage dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetText(AccountTextBox.Text);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
        }

        /// <summary>
        /// 复制密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Windows.ApplicationModel.DataTransfer.DataPackage dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetText(PasswordTextBox.Password);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
        }

        /// <summary>
        /// 显示/隐藏密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.PasswordRevealMode = PasswordTextBox.PasswordRevealMode == PasswordRevealMode.Visible ? PasswordRevealMode.Hidden : PasswordRevealMode.Visible;
            if (PasswordTextBox.PasswordRevealMode==PasswordRevealMode.Visible)
            {
                HidePasswordIcon.Visibility = Visibility.Visible;
                ShowPasswordIcon.Visibility = Visibility.Collapsed;
            }
            else
            {
                HidePasswordIcon.Visibility = Visibility.Collapsed;
                ShowPasswordIcon.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 跳转网址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            if (ShowPassword.Website != "")
            {
                try
                {
                    WrongWebsiteTextBlock.Text = "正在跳转";
                    await Windows.System.Launcher.LaunchUriAsync(new Uri(ShowPassword.Website));
                }
                catch
                {
                    WrongWebsiteTextBlock.Text = "网址错误，请检查输入网址的是否完整";
                }
            }
            else
            {
                WrongWebsiteTextBlock.Text = "没有添加网址";
            }
        }

        /// <summary>
        /// 点击收藏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Click_3(object sender, RoutedEventArgs e)
        {
            if (ShowPassword.IsFavorite == true)
            {
                FavoriteIcon.Visibility = Visibility.Visible;
                UnFavoriteIcon.Visibility = Visibility.Collapsed;
                ShowPassword.IsFavorite = false;
            }
            else
            {
                FavoriteIcon.Visibility = Visibility.Collapsed;
                UnFavoriteIcon.Visibility = Visibility.Visible;
                ShowPassword.IsFavorite = true;
            }
            PasswordHelper.SaveData();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Click_4(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                FlyoutBase.ShowAttachedFlyout(element);
            }
        }

        /// <summary>
        /// 确认删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteFlyout.Hide();
            PasswordHelper._data.Remove(ShowPassword);
            PasswordHelper.SaveData();
            HomePage.Home.HomeFrame.Navigate(typeof(PasswordPage));
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Click_5(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(EditingPage), ShowPassword);
        }
    }
}
