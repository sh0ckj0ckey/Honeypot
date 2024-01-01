using System;
using System.Collections.Generic;
using System.Text;
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
        private Random _random = new Random();

        private static char[] _letterArray = { 'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
                                               'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z' };
        private static char[] _numberArray = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private static char[] _symbolArray = { '!', '@', '#', '$', '%', '&', '*' };

        private int _rotateIndex = 0;
        private float[] _nextRotate = { 240, -480, 600, -480, 600, -480 };

        public RandomPage()
        {
            this.InitializeComponent();
        }

        private void OnClickGenerate(object sender, RoutedEventArgs e)
        {
            try
            {
                RandomImage.Rotation += _nextRotate[_rotateIndex % 6];
                _rotateIndex = (_rotateIndex + 1) % 6;
            }
            catch { }

            try
            {
                GeneratedTextBox.Text = GeneratePassword(LetterToggle.IsOn, NumberToggle.IsOn, SymbolToggle.IsOn, Convert.ToInt32(PasswordLengthSlider.Value));

                // 自动复制
                Windows.ApplicationModel.DataTransfer.DataPackage dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
                dataPackage.SetText(GeneratedTextBox.Text);
                Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
            }
            catch { }
        }

        private string GeneratePassword(bool letter, bool number, bool symbol, int length)
        {
            List<char> randomList = new List<char>();
            try
            {
                if (letter)
                {
                    randomList.AddRange(_letterArray);
                }
                if (number)
                {
                    randomList.AddRange(_numberArray);
                }
                if (symbol)
                {
                    randomList.AddRange(_symbolArray);
                }

                int arrayCount = randomList.Count;

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < length; i++)
                {
                    int index = _random.Next(arrayCount);
                    sb.Append(randomList[index]);
                }
                return sb.ToString();
            }
            catch { }
            finally
            {
                randomList.Clear();
                randomList = null;
            }
            return "";
        }

    }
}
