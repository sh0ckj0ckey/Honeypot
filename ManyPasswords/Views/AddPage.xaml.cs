using System;
using System.Collections.Generic;
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
    public sealed partial class AddPage : Page
    {
        public static AddPage Add = null;

        public AddPage()
        {
            this.InitializeComponent();
            Add = this;
            AddFrame.Navigate(typeof(BlankPage));
            if (App.AppSettingContainer.Values["Theme"] == null || App.AppSettingContainer.Values["Theme"].ToString() == "Light")
            {
                AddingGridView.Opacity = 1;
            }
            else
            {
                AddingGridView.Opacity = 0.7;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddingGridView.SelectedIndex = -1;
            AddFrame.Navigate(typeof(AddingPage), null);
        }

        private void AddingGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AddingGridView.SelectedIndex >= 0)
            {

                AddFrame.Navigate(typeof(AddingPage), MainPage.BuildIn[AddingGridView.SelectedIndex]);
            }
            else
            {
                AddFrame.Navigate(typeof(AddingPage), null);
            }
        }
    }
}
