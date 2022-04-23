using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace ManyPasswords
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public static HomePage Home = null;

        public HomePage()
        {
            this.InitializeComponent();
            Home = this;
            LockAppButton.IsEnabled = App.AppSettingContainer.Values["Password"] == null ? false : true;
            ShowKeyboardTipCheckBox.IsChecked = App.AppSettingContainer.Values["KeyboardTip"] == null || App.AppSettingContainer.Values["KeyboardTip"].ToString() == "true" ? true : false;
            WindowsHelloToggleSwitch.IsOn = App.AppSettingContainer.Values["WindowsHello"] == null || App.AppSettingContainer.Values["WindowsHello"].ToString() == "off" ? false : true;
            //ShowWelcomeCheckBox.IsChecked = App.AppSettingContainer.Values["WelcomePage"] == null || App.AppSettingContainer.Values["WelcomePage"].ToString() == "true" ? true : false;
            if (App.AppSettingContainer.Values["Theme"] == null || App.AppSettingContainer.Values["Theme"].ToString() == "Light")
            {
                Switch2Light();
            }
            else
            {
                Switch2Dark();
            }

            MenuListView.SelectedIndex = 0;

            //if (App.AppSettingContainer.Values["WelcomePage"] == null || App.AppSettingContainer.Values["WelcomePage"].ToString() == "true")
            //{
            //    HomeFrame.Navigate(typeof(WelcomePage));
            //}
            //else
            //{
            //    MenuListView.SelectedIndex = 0;
            //    //HomeFrame.Navigate(typeof(PasswordPage));
            //}
        }

        /// <summary>
        /// 锁定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LoginPage));
        }

        /// <summary>
        /// 切换夜间模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.RequestedTheme == ElementTheme.Dark)
            {
                Switch2Light();
            }
            else
            {
                Switch2Dark();
            }
        }

        /// <summary>
        /// 切换到白天主题的方法
        /// </summary>
        public void Switch2Light()
        {
            this.RequestedTheme = ElementTheme.Light;
            MenuAcrylic.TintColor = Colors.Transparent;
            LightTitleBarButton();
            Name2TextBlock.Text = "夜晚";
            SwitchDarkTextBlock.Visibility = Visibility.Visible;
            SwitchLightTextBlock.Visibility = Visibility.Collapsed;
            if (AddingPage.Adding != null)
            {
                AddingPage.Adding.PhotoPanel.Opacity = 1;
            }
            if (AddPage.Add != null)
            {
                AddPage.Add.AddingGridView.Opacity = 1;
            }
            if (DetailsPage.Current != null)
            {
                DetailsPage.Current.PhotoPanel.Opacity = 1;
            }
            if (EditingPage.Editing != null)
            {
                EditingPage.Editing.PhotoPanel.Opacity = 1;
            }
            if (PasswordPage.Password != null)
            {
                PasswordPage.Password.PasswordsSemanticZoom.Opacity = 1;
            }
            ThemeLighRadioButtont.IsChecked = true;
            App.AppSettingContainer.Values["Theme"] = "Light";
        }

        /// <summary>
        /// 切换到夜间主题的方法
        /// </summary>
        public void Switch2Dark()
        {
            this.RequestedTheme = ElementTheme.Dark;
            MenuAcrylic.TintColor = Colors.Black;
            DarkTitleBarButton();
            Name2TextBlock.Text = "白天";
            SwitchDarkTextBlock.Visibility = Visibility.Collapsed;
            SwitchLightTextBlock.Visibility = Visibility.Visible;
            if (AddingPage.Adding != null)
            {
                AddingPage.Adding.PhotoPanel.Opacity = 0.7;
            }
            if (AddPage.Add != null)
            {
                AddPage.Add.AddingGridView.Opacity = 0.8;
            }
            if (DetailsPage.Current != null)
            {
                DetailsPage.Current.PhotoPanel.Opacity = 0.7;
            }
            if (EditingPage.Editing != null)
            {
                EditingPage.Editing.PhotoPanel.Opacity = 0.7;
            }
            if (PasswordPage.Password != null)
            {
                PasswordPage.Password.PasswordsSemanticZoom.Opacity = 0.8;
            }
            ThemeDarkRadioButton.IsChecked = true;
            App.AppSettingContainer.Values["Theme"] = "Dark";
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SettingGrid.Visibility = Visibility.Visible;
            SettingGridPopIn.Begin();
            SettingPivot.SelectedIndex = 0;
        }

        /// <summary>
        /// 关闭应用设置窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SettingGridPopOut.Begin();
        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            SettingGridPopOut.Begin();
        }
        private void SettingGridFadeOut_Completed(object sender, object e)
        {
            SettingGrid.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 导航
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (MenuListView.SelectedIndex)
            {
                case 0:
                    HomeFrame.Navigate(typeof(PasswordPage));
                    break;
                case 1:
                    HomeFrame.Navigate(typeof(FavoritePage));
                    break;
                case 2:
                    HomeFrame.Navigate(typeof(AddPage));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 打开系统设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:signinoptions"));
        }

        #region 显示按钮名字的动画
        private void Button_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Name1TextBlock.Visibility = Visibility.Visible;
            ShowButton1Name.Begin();
        }

        private void Button_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            HideButton1Name.Begin();
        }

        private void Button_PointerEntered_1(object sender, PointerRoutedEventArgs e)
        {
            Name2TextBlock.Visibility = Visibility.Visible;
            ShowButton2Name.Begin();
        }

        private void Button_PointerExited_1(object sender, PointerRoutedEventArgs e)
        {
            HideButton2Name.Begin();
        }

        private void Button_PointerEntered_2(object sender, PointerRoutedEventArgs e)
        {
            Name3TextBlock.Visibility = Visibility.Visible;
            ShowButton3Name.Begin();
        }

        private void Button_PointerExited_2(object sender, PointerRoutedEventArgs e)
        {
            HideButton3Name.Begin();
        }
        #endregion

        private async void RatingControl_ValueChanged(RatingControl sender, object args)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9NLZPBCS0F5C"));
        }

        /// <summary>
        /// 修改标题栏按钮字体颜色
        /// </summary>
        public void DarkTitleBarButton()
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonForegroundColor = Colors.White;
        }
        public void LightTitleBarButton()
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonForegroundColor = Colors.Black;
        }

        /// <summary>
        /// 确定修改密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (Regex.IsMatch(NewLockPasswordBox.Text, "^[0-9]{6}$"))
            {
                App.AppSettingContainer.Values["Password"] = NewLockPasswordBox.Text;
                SuccessNewPasswordTextBlock.Visibility = Visibility.Visible;
                FailNewPasswordTextBlock.Visibility = Visibility.Collapsed;
                LockAppButton.IsEnabled = true;
            }
            else
            {
                SuccessNewPasswordTextBlock.Visibility = Visibility.Collapsed;
                FailNewPasswordTextBlock.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 取消修改密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            NewLockPasswordBox.Text = "";
            SuccessNewPasswordTextBlock.Visibility = Visibility.Collapsed;
            FailNewPasswordTextBlock.Visibility = Visibility.Collapsed;
        }

        private void ThemeLighRadioButtont_Checked(object sender, RoutedEventArgs e)
        {
            Switch2Light();
        }

        private void ThemeDarkRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Switch2Dark();
        }

        private void ShowKeyboardTipCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            App.AppSettingContainer.Values["KeyboardTip"] = "true";
        }

        private void ShowWelcomeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            App.AppSettingContainer.Values["WelcomePage"] = "true";
        }

        private void ShowKeyboardTipCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            App.AppSettingContainer.Values["KeyboardTip"] = "false";
        }

        private void ShowWelcomeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            App.AppSettingContainer.Values["WelcomePage"] = "false";
        }

        /// <summary>
        /// 开关 Windows Hello 验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (WindowsHelloToggleSwitch.IsOn)
            {
                if (App.AppSettingContainer.Values["Password"] != null && App.AppSettingContainer.Values["Password"].ToString().Length == 6)
                {
                    DidntSetPasswordTextBlock.Visibility = Visibility.Collapsed;
                    App.AppSettingContainer.Values["WindowsHello"] = "on";
                }
                else
                {
                    DidntSetPasswordTextBlock.Visibility = Visibility.Visible;
                    App.AppSettingContainer.Values["WindowsHello"] = "off";
                }
            }
            else
            {
                DidntSetPasswordTextBlock.Visibility = Visibility.Collapsed;
                App.AppSettingContainer.Values["WindowsHello"] = "off";
            }
        }
    }
}
