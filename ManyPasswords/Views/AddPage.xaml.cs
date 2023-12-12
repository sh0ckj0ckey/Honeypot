using Honeypot.ViewModel;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Honeypot
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AddPage : Page
    {
        PasswordViewModel ViewModel = null;

        public AddPage()
        {
            try
            {
                this.InitializeComponent();
                ViewModel = PasswordViewModel.Instance;
                ViewModel.InitBuildInAccounts();
                AddFrame.Navigate(typeof(BlankPage));

                AddingGrid.Translation += new System.Numerics.Vector3(0, 0, 36);
            }
            catch { }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddFrame.Navigate(typeof(AddingPage), null);
            }
            catch { }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn)
                {
                    if (btn.DataContext is Models.BuildinItem item)
                    {
                        AddFrame.Navigate(typeof(AddingPage), item);
                    }
                }
            }
            catch { }
        }
    }
}
