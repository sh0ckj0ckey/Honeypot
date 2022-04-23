using ManyPasswords.Models;
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
        public List<PasswordsGroup> itemList = new List<PasswordsGroup>();
        public List<PasswordItem> list = new List<PasswordItem>();
        public static PasswordItem Selected = null;
        public static PasswordPage Password = null;
        List<PasswordItem> suggestions = new List<PasswordItem>();
        public PasswordPage()
        {
            this.InitializeComponent();
            Password = this;
            PasswordFrame.Navigate(typeof(BlankPage));
            try
            {
                //itemList = (from item in PasswordHelper._data group item by item.sFirstLetter into newItems select new PasswordsInGroup { Key = newItems.Key, PasswordsContent = newItems.ToList() }).OrderBy(x => x.Key).ToList();
            }
            catch { }
            if (itemList.Count == 0)
            {
                NullStackPanel.Visibility = Visibility.Visible;
                SemanticGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                foreach (var group in itemList)
                {
                    foreach (var password in group.vPasswords)
                    {
                        list.Add(password);
                    }
                }
                NullStackPanel.Visibility = Visibility.Collapsed;
                SemanticGrid.Visibility = Visibility.Visible;
            }
            if (App.AppSettingContainer.Values["Theme"] == null || App.AppSettingContainer.Values["Theme"].ToString() == "Light")
            {
                PasswordsSemanticZoom.Opacity = 1;
            }
            else
            {
                PasswordsSemanticZoom.Opacity = 0.7;
            }
        }

        /// <summary>   
        /// 添加账号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HomePage.Home.MenuListView.SelectedIndex = 2;
        }

        /// <summary>
        /// 选中一项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //if (InView.SelectedIndex < 0)
            //{
            //    PasswordFrame.Navigate(typeof(BlankPage));
            //    return;
            //}
            Selected = (PasswordItem)e.ClickedItem;
            //Selected = list[InView.SelectedIndex];
            PasswordFrame.Navigate(typeof(DetailsPage));
        }

        private void InView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            //ListViewFlyout.ShowAt(InView, e.GetPosition(this.InView));
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            //suggestions.Clear();
            //suggestions = PasswordHelper._data.Where(p => (p.Name.StartsWith(sender.Text, StringComparison.CurrentCultureIgnoreCase) || p.Account.StartsWith(sender.Text, StringComparison.CurrentCultureIgnoreCase))).ToList();
            //SearchAutoSuggestBox.ItemsSource = suggestions;
        }

        /// <summary>
        /// 搜索账号功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void SearchAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            PasswordItem onePassword = (PasswordItem)args.SelectedItem;
            SearchAutoSuggestBox.Text = onePassword.sName.StartsWith(SearchAutoSuggestBox.Text, StringComparison.CurrentCultureIgnoreCase) ? onePassword.sName : onePassword.sAccount;
            Selected = onePassword;
            InView.SelectedIndex = -1;
            PasswordFrame.Navigate(typeof(DetailsPage));
        }
    }
}
