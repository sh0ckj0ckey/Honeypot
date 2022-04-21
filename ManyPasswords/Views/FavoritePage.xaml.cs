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
    public sealed partial class FavoritePage : Page
    {
        public ObservableCollection<OnePassword> FavoritesCollection = new ObservableCollection<OnePassword>();
        public static FavoritePage Favorite = null;
        public FavoritePage()
        {
            this.InitializeComponent();
            Favorite = this;
            foreach (OnePassword item in PasswordHelper._data)
            {
                if (item.IsFavorite)
                {
                    FavoritesCollection.Add(item);
                }
            }
        }

        /// <summary>
        /// 点击取消收藏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            OnePassword clickedPassword = (OnePassword)btn.DataContext;
            btn.Content = clickedPassword.IsFavorite == true ? "\uE006" : "\uE007";
            clickedPassword.IsFavorite = clickedPassword.IsFavorite == true ? false : true;
        }
    }
}
