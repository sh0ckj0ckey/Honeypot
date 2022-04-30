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

        // 搜索建议列表
        private ObservableCollection<PasswordItem> vSearchSuggestions = new ObservableCollection<PasswordItem>();

        public PasswordPage()
        {
            try
            {
                this.InitializeComponent();
                Current = this;

                ViewModel = PasswordViewModel.Instance;

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
                        SearchSuggestPasswords();
                    };
                }
            }
            catch { }
        }

        private void SearchSuggestPasswords()
        {
            try
            {
                vSearchSuggestions.Clear();
                //suggestions = PasswordHelper._data.Where(p => (p.Name.StartsWith(sender.Text, StringComparison.CurrentCultureIgnoreCase) || p.Account.StartsWith(sender.Text, StringComparison.CurrentCultureIgnoreCase))).ToList();
                SearchAutoSuggestBox.ItemsSource = vSearchSuggestions;
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
                    SearchAutoSuggestBox.Text = password.sName.StartsWith(SearchAutoSuggestBox.Text, StringComparison.CurrentCultureIgnoreCase) ? password.sName : password.sAccount;
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
