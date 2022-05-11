using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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
    public sealed partial class RandomPasswordPage : Page
    {
        private Random _random = new Random();

        private static char[] _letterArray = { 'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
                                               'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z' };
        private static char[] _numberArray = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private static char[] _symbolArray = { '!', '@', '#', '$', '%', '&', '*' };

        public RandomPasswordPage()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RandomImage.Rotation = (RandomImage.Rotation + 120) % 360;
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
            randomList.Clear();
            randomList = null;
            return "";
        }
    }
}
