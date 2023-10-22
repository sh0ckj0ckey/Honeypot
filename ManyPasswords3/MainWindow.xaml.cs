// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Diagnostics;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using Windows.UI.ViewManagement;
using WinUIEx;
using ManyPasswords3.ViewModels;
using ManyPasswords3.Helpers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ManyPasswords3
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
            this.AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/Icon/manypasswords.ico"));
            this.PersistenceId = "ManyPasswordsMainWindow";
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(AppTitleBar);

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
            //MainFrame.Navigate(typeof());

            // 处理系统的返回键和退出键
            MainFrame.KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu, OnGoBackKeyboardAcceleratorInvoked));
            MainFrame.KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack, null, OnGoBackKeyboardAcceleratorInvoked));
            MainFrame.KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.XButton1, null, OnGoBackKeyboardAcceleratorInvoked));
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

                TitleBarHelper.UpdateTitleBar(App.MainWindow, isLight ? ElementTheme.Light : ElementTheme.Dark);

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

        #region Go Back & Hide

        private KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers, Action<KeyboardAccelerator, KeyboardAcceleratorInvokedEventArgs> callback)
        {
            var keyboardAccelerator = new KeyboardAccelerator() { Key = key };

            if (modifiers.HasValue)
            {
                keyboardAccelerator.Modifiers = modifiers.Value;
            }

            keyboardAccelerator.Invoked += (s, args) => callback?.Invoke(s, args);

            return keyboardAccelerator;
        }

        private void OnGoBackKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            args.Handled = TryGoBack();
        }

        private bool TryGoBack()
        {
            try
            {
                if (!MainFrame.CanGoBack) return false;
                MainFrame.GoBack();
                return true;
            }
            catch { }
            return false;
        }

        #endregion

    }
}
