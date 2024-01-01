// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using System.IO;
using Honeypot.ViewModels;
using Honeypot.Views;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Windows.UI.ViewManagement;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Honeypot
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WindowEx
    {
        private UISettings _uiSettings;

        private Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue = null;

        public MainWindow()
        {
            this.InitializeComponent();
            this.SystemBackdrop = MainViewModel.Instance.AppSettings.BackdropIndex == 1 ? new Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop() : new Microsoft.UI.Xaml.Media.MicaBackdrop();
            this.AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/Icon/Honeypot.ico"));
            this.PersistenceId = "HoneypotMainWindow";
            this.ExtendsContentIntoTitleBar = true;
            //this.SetTitleBar(AppTitleBar);

            _dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

            MainViewModel.Instance.ActSwitchAppTheme = this.SwitchAppTheme;
            MainViewModel.Instance.ActChangeBackdrop = () =>
            {
                this.SystemBackdrop = MainViewModel.Instance.AppSettings.BackdropIndex == 1 ?
                                      new Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop() :
                                      new Microsoft.UI.Xaml.Media.MicaBackdrop();
            };

            // 监听系统主题变化
            ListenThemeColorChange();
        }

        /// <summary>
        /// MainFrame 加载完成后，导航到首页，注册快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMainFrameLoaded(object sender, RoutedEventArgs e)
        {
            // 初始导航页面
            MainFrame.Navigate(typeof(MainPage));
        }

        /// <summary>
        /// 主窗口关闭后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnMainWindowClosed(object sender, WindowEventArgs args)
        {
            Application.Current.Exit();
        }

        /// <summary>
        /// 监听系统颜色设置变更，处理主题为"跟随系统"时主题切换
        /// </summary>
        private void ListenThemeColorChange()
        {
            _uiSettings = new UISettings();
            _uiSettings.ColorValuesChanged += (s, args) =>
            {
                if (MainViewModel.Instance.AppSettings.AppearanceIndex == 0)
                {
                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        SwitchAppTheme();
                    });
                }
            };
        }

        /// <summary>
        /// 切换应用程序的主题
        /// </summary>
        private void SwitchAppTheme()
        {
            try
            {
                // 设置标题栏颜色
                bool isLight = true;
                if (MainViewModel.Instance.AppSettings.AppearanceIndex == 0) // 主题 0-System 1-Dark 2-Light
                {
                    var color = _uiSettings?.GetColorValue(UIColorType.Foreground) ?? Colors.Black;
                    var g = color.R * 0.299 + color.G * 0.587 + color.B * 0.114;
                    isLight = g < 100; // g越小，颜色越深
                }
                else
                {
                    isLight = MainViewModel.Instance.AppSettings.AppearanceIndex == 2;
                }

                // 修改标题栏按钮颜色
                // TitleBarHelper.UpdateTitleBar(App.MainWindow, isLight ? ElementTheme.Light : ElementTheme.Dark);
                var titleBar = App.MainWindow.AppWindow.TitleBar;
                // Set active window colors
                // Note: No effect when app is running on Windows 10 since color customization is not supported.
                titleBar.ForegroundColor = isLight ? Colors.Black : Colors.White;
                titleBar.BackgroundColor = Colors.Transparent;
                titleBar.ButtonForegroundColor = isLight ? Colors.Black : Colors.White;
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonHoverForegroundColor = isLight ? Colors.Black : Colors.White;
                titleBar.ButtonHoverBackgroundColor = isLight ? Windows.UI.Color.FromArgb(10, 0, 0, 0) : Windows.UI.Color.FromArgb(16, 255, 255, 255);
                titleBar.ButtonPressedForegroundColor = isLight ? Colors.Black : Colors.White;
                titleBar.ButtonPressedBackgroundColor = isLight ? Windows.UI.Color.FromArgb(08, 0, 0, 0) : Windows.UI.Color.FromArgb(10, 255, 255, 255);

                // Set inactive window colors
                // Note: No effect when app is running on Windows 10 since color customization is not supported.
                titleBar.InactiveForegroundColor = Colors.Gray;
                titleBar.InactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveForegroundColor = Colors.Gray;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

                // 设置应用程序颜色
                if (App.MainWindow.Content is FrameworkElement rootElement)
                {
                    if (MainViewModel.Instance.AppSettings.AppearanceIndex == 1)
                    {
                        rootElement.RequestedTheme = ElementTheme.Dark;
                    }
                    else if (MainViewModel.Instance.AppSettings.AppearanceIndex == 2)
                    {
                        rootElement.RequestedTheme = ElementTheme.Light;
                    }
                    else
                    {
                        rootElement.RequestedTheme = ElementTheme.Default;
                    }
                }
            }
            catch { }
        }
    }
}
