using Honeypot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Honeypot.Controls
{
    public sealed partial class MigrateControl : UserControl
    {
        private MainViewModel _viewModel = null;

        public MigrateControl()
        {
            this.InitializeComponent();
            _viewModel = MainViewModel.Instance;
        }

        public void ResetView()
        {
            MainViewModel.Instance.MigrateState = Models.MigrateStepEnum.Welcome;
        }

        private void OnClickMigrate(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.MigratePasswords();
        }

        private void OnClickDismiss(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.ActInvokeMigrater?.Invoke(false);
        }

        private void OnClickFinishMigrate(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.ShowMigrater = false;
            MainViewModel.Instance.ActInvokeMigrater?.Invoke(false);
        }
    }
}
