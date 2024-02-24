using System;
using System.Collections.Generic;
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
