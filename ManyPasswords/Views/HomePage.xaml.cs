using ManyPasswords.ViewModel;
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
        public static HomePage Current = null;
        PasswordViewModel ViewModel = null;

        public HomePage()
        {
            this.InitializeComponent();
            Current = this;
            //ShowKeyboardTipCheckBox.IsChecked = App.AppSettingContainer.Values["KeyboardTip"] == null || App.AppSettingContainer.Values["KeyboardTip"].ToString() == "true" ? true : false;
            WindowsHelloToggleSwitch.IsOn = App.AppSettingContainer.Values["WindowsHello"] == null || App.AppSettingContainer.Values["WindowsHello"].ToString() == "off" ? false : true;
            //ShowWelcomeCheckBox.IsChecked = App.AppSettingContainer.Values["WelcomePage"] == null || App.AppSettingContainer.Values["WelcomePage"].ToString() == "true" ? true : false;

            ViewModel = PasswordViewModel.Instance;

            FrameShadow.Receivers.Add(SideMenuGrid);
            HomeFrame.Translation += new System.Numerics.Vector3(0, 0, 36);

            SettingShadow.Receivers.Add(BackgroundRectangle);
            SettingPop.Translation += new System.Numerics.Vector3(0, 0, 36);

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
            try
            {
                ViewModel.LockApp();
            }
            catch { }
        }

        /// <summary>
        /// 切换夜间模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (ViewModel.eAppTheme == ElementTheme.Dark)
            {
                Switch2Light();
            }
            else
            {
                Switch2Dark();
            }
            ViewModel.sHoverTipsText = ViewModel.eAppTheme == ElementTheme.Light ?
                            "夜间模式：已关闭" : "夜间模式：已打开";
        }

        /// <summary>
        /// 切换到白天主题
        /// </summary>
        public void Switch2Light()
        {
            try
            {
                ViewModel.eAppTheme = ElementTheme.Light;
                MenuAcrylic.TintColor = Colors.Transparent;

                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                titleBar.ButtonForegroundColor = Colors.Black;

                App.AppSettingContainer.Values["Theme"] = "Light";
            }
            catch { }
        }

        /// <summary>
        /// 切换到夜间主题
        /// </summary>
        public void Switch2Dark()
        {
            try
            {
                ViewModel.eAppTheme = ElementTheme.Dark;
                MenuAcrylic.TintColor = Colors.Black;

                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                titleBar.ButtonForegroundColor = Colors.White;

                App.AppSettingContainer.Values["Theme"] = "Dark";
            }
            catch { }
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

        private void ThemeLighRadioButtont_Checked(object sender, RoutedEventArgs e)
        {
            Switch2Light();
        }

        private void ThemeDarkRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Switch2Dark();
        }

        /// <summary>
        /// 开关 Windows Hello 验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            //if (WindowsHelloToggleSwitch.IsOn)
            //{
            //    if (App.AppSettingContainer.Values["Password"] != null && App.AppSettingContainer.Values["Password"].ToString().Length == 6)
            //    {
            //        DidntSetPasswordTextBlock.Visibility = Visibility.Collapsed;
            //        App.AppSettingContainer.Values["WindowsHello"] = "on";
            //    }
            //    else
            //    {
            //        DidntSetPasswordTextBlock.Visibility = Visibility.Visible;
            //        App.AppSettingContainer.Values["WindowsHello"] = "off";
            //    }
            //}
            //else
            //{
            //    DidntSetPasswordTextBlock.Visibility = Visibility.Collapsed;
            //    App.AppSettingContainer.Values["WindowsHello"] = "off";
            //}
        }

        /// <summary>
        /// 侧边栏下方按钮 Hover
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPointerEnter(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn)
                {
                    string tag = btn.Tag?.ToString();
                    if (tag == "lock")
                    {
                        ViewModel.sHoverTipsText = "锁定";
                    }
                    else if (tag == "theme")
                    {
                        ViewModel.sHoverTipsText = ViewModel.eAppTheme == ElementTheme.Light ?
                            "夜间模式：已关闭" : "夜间模式：已打开";
                    }
                    else if (tag == "setting")
                    {
                        ViewModel.sHoverTipsText = "设置";
                    }
                }
                EnterButtonStoryboard.Begin();
            }
            catch { }
        }

        private void OnPointerExit(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                LeaveButtonStoryboard.Begin();
            }
            catch { }
        }
    }
}
