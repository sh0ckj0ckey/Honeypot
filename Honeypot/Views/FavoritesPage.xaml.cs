using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Honeypot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Honeypot.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FavoritesPage : Page
    {
        private MainViewModel MainViewModel = null;

        public FavoritesPage()
        {
            this.InitializeComponent();

            MainViewModel = MainViewModel.Instance;
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                if (sender is Grid grid)
                {
                    var passwordElement = grid.FindName("PasswordStackPanel");
                    if (passwordElement is StackPanel passwordStackPanel)
                    {
                        passwordStackPanel.Visibility = Visibility.Visible;
                    }

                    var justNameElement = grid.FindName("JustNameStackPanel");
                    if (justNameElement is StackPanel justNameStackPanel)
                    {
                        justNameStackPanel.Opacity = 0.0;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                if (sender is Grid grid)
                {
                    var passwordElement = grid.FindName("PasswordStackPanel");
                    if (passwordElement is StackPanel passwordStackPanel)
                    {
                        passwordStackPanel.Visibility = Visibility.Collapsed;
                    }

                    var justNameElement = grid.FindName("JustNameStackPanel");
                    if (justNameElement is StackPanel justNameStackPanel)
                    {
                        justNameStackPanel.Opacity = 1.0;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
