using NChinese.Imm;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 好多密码_UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class EditingPage : Page
    {
        public static string UploadPicName = "ms-appx:///Assets/BuildInIcon/default.jpg";
        private string desiredName = "";
        private char typingFirstLetter = '#';
        public static EditingPage Editing = null;
        public OnePassword ShowingPassword = null;

        public EditingPage()
        {
            this.InitializeComponent();
            Editing = this;
            if (App.AppSettingContainer.Values["Theme"] == null || App.AppSettingContainer.Values["Theme"].ToString() == "Light")
            {
                PhotoPanel.Opacity = 1;
            }
            else
            {
                PhotoPanel.Opacity = 0.7;
            }
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
                ShowingPassword= (OnePassword)e.Parameter;
                if (ShowingPassword != null)
                {
                    UploadPicName = ShowingPassword.Picture;
                    PhotoImageBrush.ImageSource = new BitmapImage(new Uri(UploadPicName, UriKind.Absolute));
                    NameTextBox.Text = ShowingPassword.Name;
                    AccountTextBox.Text = ShowingPassword.Account;
                    PasswordTextBox.Text = ShowingPassword.Password;
                    FavoriteCheckBox.IsChecked = ShowingPassword.IsFavorite;
                    LinkTextBox.Text = ShowingPassword.Website;
                    BioTextBox.Text = ShowingPassword.Info;
                    TitleTextBlock.Text = ShowingPassword.ImageName;
                    this.typingFirstLetter = ShowingPassword.FirstLetter;
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
            this.Frame.GoBack();
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
                if (LinkTextBox.Text == "")
                {
                    newPassword.Website = "未添加";
                }
                if (FavoriteCheckBox.IsChecked == false)
                {
                    newPassword.IsFavorite = false;
                }
                else
                {
                    newPassword.IsFavorite = true;
                }
                PasswordHelper._data.Remove(ShowingPassword);
                PasswordHelper._data.Add(newPassword);
                PasswordHelper.SaveData();
                desiredName = "";
                HomePage.Home.HomeFrame.Navigate(typeof(PasswordPage));
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
            if (NameTextBox.Text == "")
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
            string[] pinyin = { "" };
            try
            {
                pinyin = zhuyinProvider.Convert(NameTextBox.Text.Trim());
            }
            catch
            {
                this.typingFirstLetter = '#';
                return;
            }
            if (pinyin.Length <= 0)
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

