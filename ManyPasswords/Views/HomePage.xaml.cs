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
            try
            {
                this.InitializeComponent();
                Current = this;
                ViewModel = PasswordViewModel.Instance;

                FrameShadow.Receivers.Add(SideMenuGrid);
                HomeFrame.Translation += new System.Numerics.Vector3(0, 0, 36);

                SettingShadow.Receivers.Add(BackgroundRectangle);
                SettingPop.Translation += new System.Numerics.Vector3(0, 0, 36);

                MenuListView.SelectedIndex = 0;
            }
            catch { }
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
            try
            {
                if (ViewModel.eAppTheme == ElementTheme.Dark)
                {
                    Switch2Light();
                }
                else
                {
                    Switch2Dark();
                }
                ViewModel.sHoverTipsText = ViewModel.eAppTheme == ElementTheme.Light ? "深色模式：已关闭" : "深色模式：已打开";
            }
            catch { }
        }

        /// <summary>
        /// 切换到白天主题
        /// </summary>
        public void Switch2Light()
        {
            try
            {
                ViewModel.eAppTheme = ElementTheme.Light;

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
            try
            {
                SettingGrid.Visibility = Visibility.Visible;
                SettingGridPopIn.Begin();
                SettingPivot.SelectedIndex = 0;
            }
            catch { }
        }

        /// <summary>
        /// 关闭应用设置窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                SettingGridPopOut.Begin();
            }
            catch { }
        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SettingGridPopOut.Begin();
            }
            catch { }
        }
        private void SettingGridFadeOut_Completed(object sender, object e)
        {
            try
            {
                SettingGrid.Visibility = Visibility.Collapsed;
            }
            catch { }
        }

        /// <summary>
        /// 导航
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
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
                    case 3:
                        HomeFrame.Navigate(typeof(RandomPasswordPage));
                        break;
                    default:
                        break;
                }
            }
            catch { }
        }

        /// <summary>
        /// 打开系统设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:signinoptions"));
            }
            catch { }
        }

        private void ThemeLighRadioButtont_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                Switch2Light();
            }
            catch { }
        }

        private void ThemeDarkRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                Switch2Dark();
            }
            catch { }
        }

        /// <summary>
        /// 开关 Windows Hello 验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.SetWindowsHelloEnable(WindowsHelloToggleSwitch.IsOn);
            }
            catch { }
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
                        ViewModel.sHoverTipsText = ViewModel.eAppTheme == ElementTheme.Light ? "深色模式：已关闭" : "深色模式：已打开";
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

        private void Button_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                ImportHoverStoryboard.Begin();
            }
            catch { }
        }

        private void Button_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                ImportLeaveStoryboard.Begin();
            }
            catch { }
        }

        // 选中深色模式降低透明度
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.bTranslucentDark = true;
                App.AppSettingContainer.Values["bTranslucentInDarkMode"] = "True";
            }
            catch { }
        }

        // 取消选中深色模式降低透明度
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.bTranslucentDark = false;
                App.AppSettingContainer.Values["bTranslucentInDarkMode"] = "False";
            }
            catch { }
        }

        // 导出文件
        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                await ViewModel.ExportPasswordsFile();
            }
            catch { }
        }
    }
}
