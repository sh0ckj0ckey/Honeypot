using ManyPasswords.Models;
using ManyPasswords.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
    public sealed partial class DetailsPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        ViewModel.PasswordViewModel ViewModel = null;

        // 隐藏密码
        private bool _bHidePassword = true;
        public bool bHidePassword
        {
            get => _bHidePassword;
            set
            {
                if (_bHidePassword != value)
                {
                    _bHidePassword = value;
                    OnPropertyChanged();
                }
            }
        }

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
        /// 这里重写OnNavigatedTo方法
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            try
            {
                base.OnNavigatedTo(e);
                bHidePassword = true;
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
        /// 跳转网址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickGotoWebsite(object sender, RoutedEventArgs e)
        {
            try
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
            catch { }
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
        private void OnClickDelete(object sender, RoutedEventArgs e)
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
        private void OnClickConfirmDelete(object sender, RoutedEventArgs e)
        {
            try
            {
                DeleteFlyout.Hide();
                ViewModel.RemovePassword(ViewModel.CurrentPassword);
                this.Frame.Navigate(typeof(BlankPage));
            }
            catch { }
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickEdit(object sender, RoutedEventArgs e)
        {
            //this.Frame.Navigate(typeof(EditingPage), ShowPassword);
        }

        // 鼠标移入，显示密码
        private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                bHidePassword = false;
            }
            catch { }
        }

        // 鼠标移出，隐藏密码
        private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                bHidePassword = true;
            }
            catch { }
        }

        // 保险期间，防止鼠标移入还是没显示密码，则点击亚克力后显示密码
        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                bHidePassword = false;
            }
            catch { }
        }
    }
}
