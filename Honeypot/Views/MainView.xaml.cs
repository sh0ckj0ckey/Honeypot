using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Honeypot.Models;
using Honeypot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel.Search;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Honeypot.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainView : Page
    {
        private MainViewModel MainViewModel = null;

        // 导航栏项的Tag对应的Page
        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("all", typeof(PasswordsPage)),
            ("favorite", typeof(FavoritesPage)),
            ("adding", typeof(AddingPage)),
            ("category", typeof(CategoriesPage)),
            ("random", typeof(RandomPage)),
            ("tips", typeof(TipsPage)),
            ("settings", typeof(SettingsPage)),
        };

        private ContentDialog _tipsContentDialog = null;

        public MainView()
        {
            this.InitializeComponent();

            MainViewModel = MainViewModel.Instance;

            MainViewModel.Instance.ActSwitchAppTheme?.Invoke();

            _tipsContentDialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "",
                Content = "",
                CloseButtonText = "我知道了"
            };

            MainViewModel.Instance.ActShowTipDialog = async (title, content) =>
            {
                _tipsContentDialog.Title = title;
                _tipsContentDialog.Content = content;
                _tipsContentDialog.XamlRoot = this.XamlRoot;
                _tipsContentDialog.RequestedTheme = this.ActualTheme;
                await _tipsContentDialog.ShowAsync();
            };
        }

        private void MainNavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // 页面发生导航时，更新侧边栏的选中项
                MainFrame.Navigated += MainFrame_Navigated;

                MainFramNavigateToPage("all", null, new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());

                // 处理系统的返回键和退出键
                MainFrame.KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu, OnGoBackKeyboardAcceleratorInvoked));
                MainFrame.KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack, null, OnGoBackKeyboardAcceleratorInvoked));
                MainFrame.KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.XButton1, null, OnGoBackKeyboardAcceleratorInvoked));
            }
            catch { }
        }

        private void MainNavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            try
            {
                if (args?.InvokedItemContainer?.Tag == null || string.IsNullOrWhiteSpace(args?.InvokedItemContainer?.Tag?.ToString())) return;

                if (args.InvokedItemContainer != null)
                {
                    var navItemTag = args.InvokedItemContainer.Tag.ToString();
                    MainFramNavigateToPage(navItemTag, args.RecommendedNavigationTransitionInfo);
                }

                // 清除返回
                MainFrame.BackStack.Clear();
                MainFrame.ForwardStack.Clear();
            }
            catch { }
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            try
            {
                if (MainFrame.SourcePageType != null)
                {
                    string tag = (_pages.FirstOrDefault(p => p.Page == e.SourcePageType)).Tag;

                    // 遍历侧栏找到匹配的选项，将侧栏的选中项对应到当前页面
                    MainNavigationBase select = null;
                    if (select is null)
                    {
                        foreach (var menuItem in MainViewModel.Instance.MainNavigationItems)
                        {
                            if (menuItem is MainNavigationItem menu && menu?.Tag?.Equals(tag) == true)
                            {
                                select = menuItem;
                                break;
                            }
                        }
                    }
                    if (select is null)
                    {
                        foreach (var footerMenuItem in MainViewModel.Instance.MainNavigationFooterItems)
                        {
                            if (tag == "settings" && footerMenuItem is MainNavigationSettingItem menu)
                            {
                                select = footerMenuItem;
                                break;
                            }
                            else if (footerMenuItem is MainNavigationItem footer && footer?.Tag?.Equals(tag) == true)
                            {
                                select = footerMenuItem;
                                break;
                            }
                        }
                    }
                    if (select is null)
                    {
                        foreach (var menuItem in MainViewModel.Instance.CategoriesOnNav)
                        {
                            if (menuItem is MainNavigationItem menu && menu?.Tag?.Equals(tag) == true)
                            {
                                select = menuItem;
                                break;
                            }
                        }
                    }
                    MainNavigationView.SelectedItem = select;
                }
            }
            catch { }
        }

        private void MainFramNavigateToPage(string navItemTag, object parameter = null, Microsoft.UI.Xaml.Media.Animation.NavigationTransitionInfo transitionInfo = null)
        {
            try
            {
                Type page = null;

                var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                page = item.Page;

                var preNavPageType = MainFrame.CurrentSourcePageType;
                if (page is not null && !Type.Equals(preNavPageType, page))
                {
                    if (parameter != null || transitionInfo != null)
                    {
                        MainFrame.Navigate(page, parameter, transitionInfo);
                    }
                    else
                    {
                        MainFrame.Navigate(page);
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
