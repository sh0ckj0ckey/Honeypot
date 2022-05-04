using ManyPasswords.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace ManyPasswords
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static bool _StartUp = true;
        public PasswordViewModel ViewModel = null;
        public MainPage()
        {
            try
            {
                ViewModel = PasswordViewModel.Instance;

                this.InitializeComponent();

                // 修改标题栏样式
                var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                coreTitleBar.ExtendViewIntoTitleBar = true;
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonForegroundColor = Colors.Black;
                titleBar.ButtonInactiveForegroundColor = Colors.Gray;

                MainFrame.Navigate(typeof(HomePage));
            }
            catch { }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_StartUp)
                {
                    _StartUp = false;
                    ViewModel.UnlockApp();
                }
            }
            catch { }
        }
    }
}
