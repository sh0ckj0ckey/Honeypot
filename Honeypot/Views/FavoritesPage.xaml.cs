using System;
using System.Diagnostics;
using Honeypot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;

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

        private void OnClickFavoritePassword(object sender, RoutedEventArgs e)
        {

        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn)
                {
                    Storyboard sb = btn.Resources["PointerEnterStoryboard"] as Storyboard;
                    sb.Begin();
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
                if (sender is Button btn)
                {
                    Storyboard sb = btn.Resources["PointerLeaveStoryboard"] as Storyboard;
                    sb.Begin();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
