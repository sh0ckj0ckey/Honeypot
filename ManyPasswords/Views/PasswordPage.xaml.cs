using ManyPasswords.Models;
using ManyPasswords.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class PasswordPage : Page
    {
        PasswordViewModel ViewModel = null;

        public List<PasswordItem> list = new List<PasswordItem>();
        public static PasswordPage Current = null;

        private DispatcherTimer timer = null;

        public PasswordPage()
        {
            try
            {
                this.InitializeComponent();
                Current = this;

                ViewModel = PasswordViewModel.Instance;

                if (ViewModel.ActNavigateToBlank == null)
                {
                    ViewModel.ActNavigateToBlank += () =>
                    {
                        PasswordFrame.Navigate(typeof(BlankPage));
                    };
                }

                FrameShadow.Receivers.Add(PasswordsListGrid);
                FrameGrid.Translation += new System.Numerics.Vector3(0, 0, 36);

                PasswordFrame.Navigate(typeof(BlankPage));
            }
            catch { }
        }

        /// <summary>
        /// 添加账号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HomePage.Current.MenuListView.SelectedIndex = 2;
            }
            catch { }
        }

        /// <summary>
        /// 选中一项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InView_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.ClickedItem is PasswordItem password)
                {
                    ViewModel.CurrentPassword = password;
                    PasswordFrame.Navigate(typeof(DetailsPage));
                }
            }
            catch { }
        }

        /// <summary>
        /// 搜索账号功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            try
            {
                if (timer == null)
                {
                    timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5) };
                    timer.Tick += (s, e) =>
                    {
                        timer.Stop();
                        string searching = SearchAutoSuggestBox.Text;
                        ViewModel.SearchSuggestPasswords(searching.Trim());
                    };
                }

                if (timer.IsEnabled)
                {
                    timer.Stop();
                }
                timer.Start();
            }
            catch { }
        }

        /// <summary>
        /// 选中搜索结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void SearchAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            try
            {
                if (args.SelectedItem is PasswordItem password)
                {
                    string autoFill = string.Empty;
                    if (password.sName.StartsWith(SearchAutoSuggestBox.Text, StringComparison.CurrentCultureIgnoreCase) || password.sName == SearchAutoSuggestBox.Text)
                    {
                        autoFill = password.sName;
                    }
                    else
                    {
                        autoFill = password.sAccount;
                    }
                    SearchAutoSuggestBox.Text = "";
                    SearchAutoSuggestBox.Text = autoFill;
                    ViewModel.CurrentPassword = password;
                    PasswordFrame.Navigate(typeof(DetailsPage));

                    try
                    {
                        InView.SelectedItem = password;
                        InView.ScrollIntoView(password);
                    }
                    catch
                    {
                        InView.SelectedIndex = -1;
                    }
                }
            }
            catch { }
        }
    }
}
