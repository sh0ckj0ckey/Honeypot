using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
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
    public sealed partial class LoginPage : Page
    {
        private string InputPassword = "";

        public LoginPage()
        {
            this.InitializeComponent();
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += Dispatcher_AcceleratorKeyActivated;
            if (App.AppSettingContainer.Values["Theme"] == null || App.AppSettingContainer.Values["Theme"].ToString() == "Light")
            {
                this.RequestedTheme = ElementTheme.Light;
            }
            else
            {
                this.RequestedTheme = ElementTheme.Dark;
            }

            if (App.AppSettingContainer.Values["WindowsHello"] == null || App.AppSettingContainer.Values["WindowsHello"].ToString() == "off")
            {
                WindowsHelloButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                WindowsHelloButton.Visibility = Visibility.Visible;
            }
        }

        private void Dispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            if (args.EventType.ToString().Contains("Down"))
            {
                switch (args.VirtualKey)
                {
                    case Windows.System.VirtualKey.Back:
                    case Windows.System.VirtualKey.Delete:
                        ShowBackground10.Begin();
                        if (InputPassword.Length <= 0)
                        {
                            return;
                        }
                        InputPassword = InputPassword.Substring(0, InputPassword.Length - 1);
                        ShowInput();
                        break;
                    case Windows.System.VirtualKey.Number0:
                    case Windows.System.VirtualKey.NumberPad0:
                        ShowBackground0.Begin();
                        InputPassword += "0";
                        ShowInput();
                        break;
                    case Windows.System.VirtualKey.Number1:
                    case Windows.System.VirtualKey.NumberPad1:
                        ShowBackground1.Begin();
                        InputPassword += "1";
                        ShowInput();
                        break;
                    case Windows.System.VirtualKey.Number2:
                    case Windows.System.VirtualKey.NumberPad2:
                        ShowBackground2.Begin();
                        InputPassword += "2";
                        ShowInput();
                        break;
                    case Windows.System.VirtualKey.Number3:
                    case Windows.System.VirtualKey.NumberPad3:
                        ShowBackground3.Begin();
                        InputPassword += "3";
                        ShowInput();
                        break;
                    case Windows.System.VirtualKey.Number4:
                    case Windows.System.VirtualKey.NumberPad4:
                        ShowBackground4.Begin();
                        InputPassword += "4";
                        ShowInput();
                        break;
                    case Windows.System.VirtualKey.Number5:
                    case Windows.System.VirtualKey.NumberPad5:
                        ShowBackground5.Begin();
                        InputPassword += "5";
                        ShowInput();
                        break;
                    case Windows.System.VirtualKey.Number6:
                    case Windows.System.VirtualKey.NumberPad6:
                        ShowBackground6.Begin();
                        InputPassword += "6";
                        ShowInput();
                        break;
                    case Windows.System.VirtualKey.Number7:
                    case Windows.System.VirtualKey.NumberPad7:
                        ShowBackground7.Begin();
                        InputPassword += "7";
                        ShowInput();
                        break;
                    case Windows.System.VirtualKey.Number8:
                    case Windows.System.VirtualKey.NumberPad8:
                        ShowBackground8.Begin();
                        InputPassword += "8";
                        ShowInput();
                        break;
                    case Windows.System.VirtualKey.Number9:
                    case Windows.System.VirtualKey.NumberPad9:
                        ShowBackground9.Begin();
                        InputPassword += "9";
                        ShowInput();
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 重写导航至此页面的代码,显示动画
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Windows.UI.Xaml.Media.Animation.NavigationTransitionInfo transition)
            {
                navigationTransition.DefaultNavigationTransitionInfo = transition;
            }
            if (App.AppSettingContainer.Values["KeyboardTip"] == null || App.AppSettingContainer.Values["KeyboardTip"].ToString() == "true")
            {
                KeyboardTipGrid.Visibility = Visibility.Visible;
            }
            else
            {
                KeyboardTipGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ShowBackground1.Begin();
            InputPassword += "1";
            ShowInput();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ShowBackground2.Begin();
            InputPassword += "2";
            ShowInput();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ShowBackground3.Begin();
            InputPassword += "3";
            ShowInput();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            ShowBackground4.Begin();
            InputPassword += "4";
            ShowInput();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            ShowBackground5.Begin();
            InputPassword += "5";
            ShowInput();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            ShowBackground6.Begin();
            InputPassword += "6";
            ShowInput();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            ShowBackground7.Begin();
            InputPassword += "7";
            ShowInput();
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            ShowBackground8.Begin();
            InputPassword += "8";
            ShowInput();
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            ShowBackground9.Begin();
            InputPassword += "9";
            ShowInput();
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            ShowBackground0.Begin();
            InputPassword += "0";
            ShowInput();
        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            ShowBackground10.Begin();
            if (InputPassword.Length <= 0)
            {
                return;
            }
            InputPassword = InputPassword.Substring(0, InputPassword.Length - 1);
            ShowInput();
        }

        /// <summary>
        /// 根据输入进行六个白点的变化
        /// </summary>
        public async void ShowInput()
        {
            switch (InputPassword.Length)
            {
                case 0:
                    Ellipse0.Fill = new SolidColorBrush(Colors.Transparent);
                    Ellipse1.Fill = new SolidColorBrush(Colors.Transparent);
                    Ellipse2.Fill = new SolidColorBrush(Colors.Transparent);
                    Ellipse3.Fill = new SolidColorBrush(Colors.Transparent);
                    Ellipse4.Fill = new SolidColorBrush(Colors.Transparent);
                    Ellipse5.Fill = new SolidColorBrush(Colors.Transparent);
                    break;
                case 1:
                    Ellipse0.Fill = new SolidColorBrush(Colors.White);
                    Ellipse1.Fill = new SolidColorBrush(Colors.Transparent);
                    Ellipse2.Fill = new SolidColorBrush(Colors.Transparent);
                    Ellipse3.Fill = new SolidColorBrush(Colors.Transparent);
                    Ellipse4.Fill = new SolidColorBrush(Colors.Transparent);
                    Ellipse5.Fill = new SolidColorBrush(Colors.Transparent);
                    break;
                case 2:
                    Ellipse0.Fill = new SolidColorBrush(Colors.White);
                    Ellipse1.Fill = new SolidColorBrush(Colors.White);
                    Ellipse2.Fill = new SolidColorBrush(Colors.Transparent);
                    Ellipse3.Fill = new SolidColorBrush(Colors.Transparent);
                    Ellipse4.Fill = new SolidColorBrush(Colors.Transparent);
                    Ellipse5.Fill = new SolidColorBrush(Colors.Transparent);
                    break;
                case 3:
                    Ellipse0.Fill = new SolidColorBrush(Colors.White);
                    Ellipse1.Fill = new SolidColorBrush(Colors.White);
                    Ellipse2.Fill = new SolidColorBrush(Colors.White);
                    Ellipse3.Fill = new SolidColorBrush(Colors.Transparent);
                    Ellipse4.Fill = new SolidColorBrush(Colors.Transparent);
                    Ellipse5.Fill = new SolidColorBrush(Colors.Transparent);
                    break;
                case 4:
                    Ellipse0.Fill = new SolidColorBrush(Colors.White);
                    Ellipse1.Fill = new SolidColorBrush(Colors.White);
                    Ellipse2.Fill = new SolidColorBrush(Colors.White);
                    Ellipse3.Fill = new SolidColorBrush(Colors.White);
                    Ellipse4.Fill = new SolidColorBrush(Colors.Transparent);
                    Ellipse5.Fill = new SolidColorBrush(Colors.Transparent);
                    break;
                case 5:
                    Ellipse0.Fill = new SolidColorBrush(Colors.White);
                    Ellipse1.Fill = new SolidColorBrush(Colors.White);
                    Ellipse2.Fill = new SolidColorBrush(Colors.White);
                    Ellipse3.Fill = new SolidColorBrush(Colors.White);
                    Ellipse4.Fill = new SolidColorBrush(Colors.White);
                    Ellipse5.Fill = new SolidColorBrush(Colors.Transparent);
                    break;
                case 6:
                    Ellipse0.Fill = new SolidColorBrush(Colors.White);
                    Ellipse1.Fill = new SolidColorBrush(Colors.White);
                    Ellipse2.Fill = new SolidColorBrush(Colors.White);
                    Ellipse3.Fill = new SolidColorBrush(Colors.White);
                    Ellipse4.Fill = new SolidColorBrush(Colors.White);
                    Ellipse5.Fill = new SolidColorBrush(Colors.White);

                    if (InputPassword == App.AppSettingContainer.Values["Password"].ToString())
                    {
                        //解锁成功
                        //AtThisPage = false;
                        this.Frame.GoBack();
                        Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -= Dispatcher_AcceleratorKeyActivated;
                    }
                    else
                    {
                        ////六个点左右震动一下并清空

                        await InputStackPanel.Offset(offsetX: -4, offsetY: 0, duration: 20, delay: 0, easingType: EasingType.Linear).StartAsync();
                        await InputStackPanel.Offset(offsetX: 4, offsetY: 0, duration: 40, delay: 20, easingType: EasingType.Linear).StartAsync();
                        await InputStackPanel.Offset(offsetX: 0, offsetY: 0, duration: 20, delay: 60, easingType: EasingType.Linear).StartAsync();

                        Ellipse0.Fill = new SolidColorBrush(Colors.Transparent);
                        Ellipse1.Fill = new SolidColorBrush(Colors.Transparent);
                        Ellipse2.Fill = new SolidColorBrush(Colors.Transparent);
                        Ellipse3.Fill = new SolidColorBrush(Colors.Transparent);
                        Ellipse4.Fill = new SolidColorBrush(Colors.Transparent);
                        Ellipse5.Fill = new SolidColorBrush(Colors.Transparent);
                        InputPassword = "";
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 使用 Windows Hello 解锁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void WindowsHelloButton_Click(object sender, RoutedEventArgs e)
        {
            switch (await Windows.Security.Credentials.UI.UserConsentVerifier.RequestVerificationAsync("验证您的身份"))
            {
                case Windows.Security.Credentials.UI.UserConsentVerificationResult.Verified:
                    this.Frame.GoBack();
                    Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -= Dispatcher_AcceleratorKeyActivated;
                    break;
                case Windows.Security.Credentials.UI.UserConsentVerificationResult.DeviceNotPresent:
                case Windows.Security.Credentials.UI.UserConsentVerificationResult.NotConfiguredForUser:
                case Windows.Security.Credentials.UI.UserConsentVerificationResult.DisabledByPolicy:
                    await new Windows.UI.Popups.MessageDialog("当前识别设备未配置或被系统策略禁用，请尝试使用密码解锁").ShowAsync();
                    WindowsHelloButton.Visibility = Visibility.Collapsed;
                    return;
                case Windows.Security.Credentials.UI.UserConsentVerificationResult.DeviceBusy:
                    //case Windows.Security.Credentials.UI.UserConsentVerificationResult.Canceled:
                    await new Windows.UI.Popups.MessageDialog("当前识别设备不可用，请尝试使用密码解锁").ShowAsync();
                    WindowsHelloButton.Visibility = Visibility.Collapsed;
                    return;
                case Windows.Security.Credentials.UI.UserConsentVerificationResult.RetriesExhausted:
                    await new Windows.UI.Popups.MessageDialog("验证失败，请尝试使用密码解锁").ShowAsync();
                    WindowsHelloButton.Visibility = Visibility.Collapsed;
                    return;
                default:
                    return;
            }
        }
    }
}
