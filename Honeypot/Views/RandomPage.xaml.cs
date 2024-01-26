using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Honeypot.Helpers;
using Honeypot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Honeypot.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RandomPage : Page
    {
        private MainViewModel MainViewModel = null;

        private int _rotateIndex = 0;

        //private float[] _nextRotate = { 240, -480, 600, -480, 600, -480 };
        private float[] _nextRotate = { 360, -360, 360, -360, 360, -360 };

        public RandomPage()
        {
            this.InitializeComponent();

            MainViewModel = MainViewModel.Instance;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                CopiedInfoBar.IsOpen = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void OnClickGenerate(object sender, RoutedEventArgs e)
        {
            try
            {
                RandomImage.Rotation += _nextRotate[_rotateIndex % 6];
                _rotateIndex = (_rotateIndex + 1) % 6;

                string password = RandomPasswordGenerator.GeneratePassword(LetterToggle.IsOn, NumberToggle.IsOn, SymbolToggle.IsOn, Convert.ToInt32(PasswordLengthSlider.Value));
                GeneratedTextBox.Text = password;

                // 自动复制
                Windows.ApplicationModel.DataTransfer.DataPackage dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
                dataPackage.SetText(password);
                Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);

                if (!MainViewModel.Instance.AppSettings.NoTipAtRandom)
                {
                    CopiedInfoBar.IsOpen = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
