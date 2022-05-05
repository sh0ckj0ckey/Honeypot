using ManyPasswords.Models;
using ManyPasswords.ViewModel;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace ManyPasswords
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DetailsPage : Page
    {
        ViewModel.PasswordViewModel ViewModel = null;
        public DetailsPage()
        {
            try
            {
                ViewModel = PasswordViewModel.Instance;
                this.InitializeComponent();

                PhotoShadow.Receivers.Add(BackgroundGrid);
                PhotoRectangle.Translation += new System.Numerics.Vector3(0, 0, 32);
            }
            catch { }
        }

        /// <summary>
        /// 复制账号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickCopyAccount(object sender, RoutedEventArgs e)
        {
            try
            {
                Windows.ApplicationModel.DataTransfer.DataPackage dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
                dataPackage.SetText(AccountTextBlock.Text);
                Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
            }
            catch { }
        }

        /// <summary>
        /// 复制密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickCopyPassword(object sender, RoutedEventArgs e)
        {
            try
            {
                Windows.ApplicationModel.DataTransfer.DataPackage dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
                dataPackage.SetText(PasswordTextBlock.Text);
                Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
            }
            catch { }
        }

        /// <summary>
        /// 显示/隐藏密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickHidePassword(object sender, RoutedEventArgs e)
        {
            try
            {
                //PasswordTextBlock.PasswordRevealMode = PasswordTextBlock.PasswordRevealMode == PasswordRevealMode.Visible ? PasswordRevealMode.Hidden : PasswordRevealMode.Visible;
                //if (PasswordTextBlock.PasswordRevealMode == PasswordRevealMode.Visible)
                //{
                //    HidePasswordIcon.Visibility = Visibility.Visible;
                //    ShowPasswordIcon.Visibility = Visibility.Collapsed;
                //}
                //else
                //{
                //    HidePasswordIcon.Visibility = Visibility.Collapsed;
                //    ShowPasswordIcon.Visibility = Visibility.Visible;
                //}
            }
            catch { }
        }

        /// <summary>
        /// 跳转网址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickGotoWebsite(object sender, RoutedEventArgs e)
        {
            //if (!string.IsNullOrEmpty(ViewModel.CurrentPassword.sWebsite.Trim()))
            //{
            //    try
            //    {
            //        WebsiteTipTextBlock.Text = "正在跳转...";
            //        await Windows.System.Launcher.LaunchUriAsync(new Uri(ViewModel.CurrentPassword.sWebsite));
            //    }
            //    catch
            //    {
            //        WebsiteTipTextBlock.Text = "网址错误，请检查输入网址的是否完整";
            //    }
            //}
            //else
            //{
            //    WebsiteTipTextBlock.Text = "没有添加网址";
            //}
        }

        /// <summary>
        /// 点击收藏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickFavorite(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.AddFavorite(ViewModel.CurrentPassword);
            }
            catch { }
        }

        /// <summary>
        /// 点击取消收藏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickDelFavorite(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.RemoveFavorite(ViewModel.CurrentPassword);
            }
            catch { }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                FrameworkElement element = sender as FrameworkElement;
                if (element != null)
                {
                    FlyoutBase.ShowAttachedFlyout(element);
                }
            }
            catch { }
        }

        /// <summary>
        /// 确认删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            //DeleteFlyout.Hide();
            //PasswordHelper._data.Remove(ShowPassword);
            //PasswordHelper.SaveData();
            //HomePage.Home.HomeFrame.Navigate(typeof(PasswordPage));
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Click_5(object sender, RoutedEventArgs e)
        {
            //this.Frame.Navigate(typeof(EditingPage), ShowPassword);
        }
    }
}
