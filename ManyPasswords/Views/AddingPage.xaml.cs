using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using NChinese;
using NChinese.Imm;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace ManyPasswords
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AddingPage : Page
    {
        public static string UploadPicName = "ms-appx:///Assets/BuildInIcon/default.jpg";
        private string desiredName = "";
        private char typingFirstLetter = '#';
        public static AddingPage Adding = null;

        public AddingPage()
        {
            this.InitializeComponent();
            Adding = this;
            if (App.AppSettingContainer.Values["Theme"] == null || App.AppSettingContainer.Values["Theme"].ToString() == "Light")
            {
                PhotoPanel.Opacity = 1;
            }
            else
            {
                PhotoPanel.Opacity = 0.7;
            }
            UploadPicName = "ms-appx:///Assets/BuildInIcon/default.jpg";
        }

        /// <summary>
        /// 这里重写OnNavigatedTo方法
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                //这个e.Parameter是获取传递过来的参数
                OnePassword creating = (OnePassword)e.Parameter;
                if (creating != null)
                {
                    UploadPicName = creating.Picture;
                    PhotoImageBrush.ImageSource = new BitmapImage(new Uri(UploadPicName, UriKind.Absolute));
                    NameTextBox.Text = creating.Name;
                    AccountTextBox.Text = creating.Account;
                    PasswordTextBox.Text = creating.Password;
                    FavoriteCheckBox.IsChecked = creating.IsFavorite;
                    LinkTextBox.Text = creating.Website;
                    BioTextBox.Text = creating.Info;
                    TitleTextBlock.Text = creating.ImageName;
                    this.typingFirstLetter = creating.FirstLetter;
                }
            }
        }

        /// <summary>
        /// 取消添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UploadPicName = "ms-appx:///Assets/BuildInIcon/default.jpg";
            AddPage.Add.AddingGridView.SelectedIndex = -1;
            this.Frame.Navigate(typeof(BlankPage));
        }

        /// <summary>
        /// 确定添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (PasswordTextBox.Text == "" || AccountTextBox.Text == "")
            {
                ToastTextBlock.Text = "账号或者密码不能为空";
                ErrorGrid.Visibility = Visibility.Visible;
                ShowErrorGrid.Begin();
            }
            else
            {
                OnePassword newPassword = new OnePassword(UploadPicName, NameTextBox.Text, BioTextBox.Text, LinkTextBox.Text, AccountTextBox.Text, PasswordTextBox.Text, /*PriorityRatingControl.Value*/0, typingFirstLetter);
                if (FavoriteCheckBox.IsChecked == false)
                {
                    newPassword.IsFavorite = false;
                }
                else
                {
                    newPassword.IsFavorite = true;
                }
                PasswordHelper._data.Add(newPassword);
                PasswordHelper.SaveData();
                AddPage.Add.AddingGridView.SelectedIndex = -1;
                desiredName = "";
                this.Frame.Navigate(typeof(BlankPage));
            }
        }
        private void DoubleAnimation_Completed(object sender, object e)
        {
            HideErrorGrid.Begin();
        }
        private void DoubleAnimation_Completed_1(object sender, object e)
        {
            ErrorGrid.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 修改图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click_2Async(object sender, RoutedEventArgs e)
        {
            //创建和自定义 FileOpenPicker
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            //选取单个文件  
            StorageFile file = await picker.PickSingleFileAsync();
            if (desiredName == "")
            {
                desiredName = DateTime.Now.Ticks + ".jpg";
            }
            StorageFolder applicationFolder = ApplicationData.Current.LocalFolder;
            StorageFolder folder = await applicationFolder.CreateFolderAsync("Pic", CreationCollisionOption.OpenIfExists);
            try
            {
                StorageFile saveFile = await file.CopyAsync(folder, desiredName, NameCollisionOption.ReplaceExisting);
            }
            catch
            {
                return;
            }
            UploadPicName = "ms-appdata:///local/Pic/" + desiredName;
            PhotoImageBrush.ImageSource = new BitmapImage(new Uri(UploadPicName, UriKind.Absolute));
        }

        /// <summary>
        /// 用户输入时记录下第一个字母
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NameTextBox.Text == "" || System.Text.RegularExpressions.Regex.IsMatch(NameTextBox.Text.Trim(), "^[0-9]"))
            {
                this.typingFirstLetter = '#';
                return;
            }
            if (System.Text.RegularExpressions.Regex.IsMatch(NameTextBox.Text.Trim(), "^[a-zA-Z]"))
            {
                this.typingFirstLetter = NameTextBox.Text[0].ToString().ToUpper()[0];
                return;
            }
            var zhuyinProvider = new ImmPinyinConversionProvider();
            string[] pinyin;
            try
            {
                pinyin = zhuyinProvider.Convert(NameTextBox.Text.Trim());
            }
            catch
            {
                this.typingFirstLetter = '#';
                return;
            }
            if (pinyin == null || pinyin.Length <= 0)
            {
                this.typingFirstLetter = '#';
                return;
            }
            try
            {
                switch (pinyin[0])
                {
                    case "ā":
                    case "á":
                    case "ǎ":
                    case "à":
                        this.typingFirstLetter = 'A';
                        break;
                    case "ō":
                    case "ó":
                    case "ǒ":
                    case "ò":
                        this.typingFirstLetter = 'O';
                        break;
                    case "ē":
                    case "é":
                    case "ě":
                    case "è":
                    case "ê":
                        this.typingFirstLetter = 'E';
                        break;
                    case "ī":
                    case "í":
                    case "ǐ":
                    case "ì":
                        this.typingFirstLetter = 'I';
                        break;
                    case "ū":
                    case "ú":
                    case "ǔ":
                    case "ù":
                        this.typingFirstLetter = 'U';
                        break;
                    case "ǖ":
                    case "ǘ":
                    case "ǚ":
                    case "ǜ":
                    case "ü":
                        this.typingFirstLetter = 'V';
                        break;
                    default:
                        this.typingFirstLetter = pinyin[0][0].ToString().ToUpper()[0];
                        break;
                }
            }
            catch
            {
                this.typingFirstLetter = '#';
            }
        }

        //private void NameTextBox_Paste(object sender, TextControlPasteEventArgs e)
        //{
        //    //ToastTextBlock.Text = "抱歉，不能粘贴";
        //    //ErrorGrid.Visibility = Visibility.Visible;
        //    //ShowErrorGrid.Begin();
        //    //e.Handled = true;
        //}
    }
}
