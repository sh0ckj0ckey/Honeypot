using ManyPasswords.Models;
using ManyPasswords.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using Windows.UI.Composition;
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

        private Style _customDialogStyle = null;

        public DetailsPage()
        {
            try
            {
                ViewModel = PasswordViewModel.Instance;
                this.InitializeComponent();

                PhotoShadow.Receivers.Add(BackgroundGrid);
                PhotoRectangle.Translation += new System.Numerics.Vector3(0, 0, 32);

                _customDialogStyle = (Style)Application.Current.Resources["CustomDialogStyle"];
            }
            catch { }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var expressionAnim = _compositor.CreateExpressionAnimation();

                // The ellipse's scale is inversely proportional to the rectangle's scale
                expressionAnim.Expression = "Vector3(1/scaleElement.Scale.X, 1/scaleElement.Scale.Y, 1)";
                expressionAnim.Target = "Scale";

                // Use SetExpressionReferenceParameter to alias a UIElement into the expression string
                expressionAnim.SetExpressionReferenceParameter("scaleElement", EditButtonGrid);

                // Start the animation on the ellipse
                DeleteButtonGrid.StartAnimation(expressionAnim);

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
                if (!string.IsNullOrEmpty(ViewModel.CurrentPassword.sWebsite.Trim()))
                {
                    try
                    {
                        GoingToEnterStoryboard.Begin();
                    }
                    catch { }
                    await Windows.System.Launcher.LaunchUriAsync(new Uri(ViewModel.CurrentPassword.sWebsite));
                    return;
                }
            }
            catch { }

            try
            {
                CantgoEnterStoryboard.Begin();
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
                ViewModel.AddFavorite(ViewModel.CurrentPassword, true);
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
                ViewModel.RemoveFavorite(ViewModel.CurrentPassword, true);
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
        private async void OnClickEdit(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.EditingTempPassword = new Models.PasswordItem(ViewModel.CurrentPassword);

                if (_customDialogStyle == null)
                {
                    _customDialogStyle = (Style)Application.Current.Resources["CustomDialogStyle"];
                }

                ContentDialog dialog = new ContentDialog();

                if (_customDialogStyle != null)
                {
                    dialog.Style = _customDialogStyle;
                }

                dialog.Title = ViewModel.CurrentPassword.sName;
                dialog.PrimaryButtonText = "保存";
                dialog.IsSecondaryButtonEnabled = false;
                dialog.CloseButtonText = "取消";
                dialog.DefaultButton = ContentDialogButton.Primary;
                dialog.Content = new Views.EditDialogContent(ViewModel.CurrentPassword);
                dialog.RequestedTheme = ViewModel.eAppTheme;

                var result = await dialog.ShowAsync();

                // 如果点击了保存，则更新信息
                if (result == ContentDialogResult.Primary)
                {
                    try
                    {
                        if (File.Exists(ViewModel.CurrentPassword.sPicture) &&
                            ViewModel.CurrentPassword.sPicture != ViewModel.EditingTempPassword.sPicture &&
                            ViewModel.CurrentPassword.sPicture.Contains("password_icon_"))
                        {
                            File.Delete(ViewModel.CurrentPassword.sPicture);
                        }
                    }
                    catch { }
                    ViewModel.CurrentPassword.sAccount = ViewModel.EditingTempPassword.sAccount;
                    ViewModel.CurrentPassword.sPassword = ViewModel.EditingTempPassword.sPassword;
                    ViewModel.CurrentPassword.sPicture = ViewModel.EditingTempPassword.sPicture;
                    ViewModel.CurrentPassword.sWebsite = ViewModel.EditingTempPassword.sWebsite;
                    ViewModel.CurrentPassword.sNote = ViewModel.EditingTempPassword.sNote;
                }
                ViewModel.EditingTempPassword = null;
            }
            catch { }
        }


        // 收藏按钮缩放动画
        private Compositor _compositor = Window.Current.Compositor;
        private SpringVector3NaturalMotionAnimation _springAnimation;

        private void CreateOrUpdateSpringAnimation(float finalValue)
        {
            try
            {
                if (_springAnimation == null)
                {
                    _springAnimation = _compositor.CreateSpringVector3Animation();
                    _springAnimation.Target = "Scale";
                    _springAnimation.DampingRatio = 0.5f;
                }

                _springAnimation.FinalValue = new Vector3(finalValue);
            }
            catch { }
        }

        private void SpringAnimationPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                // Scale up to 1.3
                CreateOrUpdateSpringAnimation(1.3f);

                (sender as UIElement).StartAnimation(_springAnimation);
            }
            catch { }
        }

        private void SpringAnimationPointerExited(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                // Scale back down to 1.0
                CreateOrUpdateSpringAnimation(1.0f);

                (sender as UIElement).StartAnimation(_springAnimation);
            }
            catch { }
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

        // 保险起见，防止鼠标移入还是没显示密码，则点击亚克力后显示密码
        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                bHidePassword = false;
            }
            catch { }
        }

        private void GoingToEnterStoryboard_Completed(object sender, object e)
        {
            try
            {
                GoingToLeaveStoryboard.Begin();
            }
            catch { }
        }

        private void CantgoEnterStoryboard_Completed(object sender, object e)
        {
            try
            {
                CantgoLeaveStoryboard.Begin();
            }
            catch { }
        }
    }
}
