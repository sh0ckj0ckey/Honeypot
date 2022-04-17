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

namespace 好多密码_UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PasswordPage : Page
    {
        public List<PasswordsInGroup> itemList = new List<PasswordsInGroup>();
        public List<OnePassword> list = new List<OnePassword>();
        public static OnePassword Selected = null;
        public static PasswordPage Password = null;
        List<OnePassword> suggestions = new List<OnePassword>();
        public PasswordPage()
        {
            this.InitializeComponent();
            Password = this;
            PasswordFrame.Navigate(typeof(BlankPage));
            try
            {
                itemList = (from item in PasswordHelper._data group item by item.FirstLetter into newItems select new PasswordsInGroup { Key = newItems.Key, PasswordsContent = newItems.ToList() }).OrderBy(x => x.Key).ToList();
            }
            catch { }
            //PasswordsCollectionViewSource.Source = itemList;
            //OutView.ItemsSource = PasswordsCollectionViewSource.View.CollectionGroups;
            //InView.ItemsSource = PasswordsCollectionViewSource.View;
            if (itemList.Count == 0)
            {
                NullStackPanel.Visibility = Visibility.Visible;
                SemanticGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                foreach (var group in itemList)
                {
                    foreach (var password in group.PasswordsContent)
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
            Selected = (OnePassword)e.ClickedItem;
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
            suggestions.Clear();
            suggestions = PasswordHelper._data.Where(p => (p.Name.StartsWith(sender.Text, StringComparison.CurrentCultureIgnoreCase) || p.Account.StartsWith(sender.Text, StringComparison.CurrentCultureIgnoreCase))).ToList();
            SearchAutoSuggestBox.ItemsSource = suggestions;
        }

        /// <summary>
        /// 搜索账号功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void SearchAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            OnePassword onePassword = (OnePassword)args.SelectedItem;
            SearchAutoSuggestBox.Text = onePassword.Name.StartsWith(SearchAutoSuggestBox.Text, StringComparison.CurrentCultureIgnoreCase) ? onePassword.Name : onePassword.Account;
            Selected = onePassword;
            InView.SelectedIndex = -1;
            PasswordFrame.Navigate(typeof(DetailsPage));
        }
    }
}
