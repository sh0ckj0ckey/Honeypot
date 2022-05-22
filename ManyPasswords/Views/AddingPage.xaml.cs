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
using ManyPasswords.Models;
using ManyPasswords.ViewModel;
using System.Runtime.CompilerServices;
using System.ComponentModel;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace ManyPasswords
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AddingPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        PasswordViewModel ViewModel = null;

        private Models.PasswordItem _AddingItem = null;
        public Models.PasswordItem AddingItem
        {
            get => _AddingItem;
            set
            {
                if (_AddingItem != value)
                {
                    _AddingItem = value;
                    OnPropertyChanged();
                }
            }
        }

        public AddingPage()
        {
            try
            {
                this.InitializeComponent();
                ViewModel = PasswordViewModel.Instance;

                PhotoShadow.Receivers.Add(ImageGrid);
                AddingImageGrid.Translation += new System.Numerics.Vector3(0, 0, 32);
            }
            catch { }
        }

        /// <summary>
        /// 这里重写OnNavigatedTo方法
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                base.OnNavigatedTo(e);
                AddingItem = null;
                AddingItem = new PasswordItem("", "", "", "ms-appx:///Assets/BuildInIcon/default.jpg", "", "", false);

                if (e.Parameter != null && e.Parameter is Models.BuildinItem buildin)
                {
                    //这个e.Parameter是获取传递过来的参数
                    AddingItem.sName = buildin.sName;
                    AddingItem.sNote = buildin.sNote;
                    AddingItem.sWebsite = buildin.sWebsite;
                    AddingItem.sPicture = buildin.sCoverImage;
                }
            }
            catch { }
        }

        /// <summary>
        /// 取消添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddingItem = null;

                this.Frame.Navigate(typeof(BlankPage));
            }
            catch { }
        }

        /// <summary>
        /// 确定添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickAdd(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(AddingItem.sName) || string.IsNullOrEmpty(AddingItem.sAccount) || string.IsNullOrEmpty(AddingItem.sPassword))
                {
                    ShowEmptyToastGrid.Begin();
                }
                else
                {
                    ViewModel.AddPassword(new PasswordItem(AddingItem));
                    AddingItem = null;

                    this.Frame.Navigate(typeof(BlankPage));
                }
            }
            catch { }
        }

        /// <summary>
        /// 修改图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnChangeImageClick(object sender, RoutedEventArgs e)
        {
            try
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
                var file = await picker.PickSingleFileAsync();

                if (file != null)
                {
                    string desiredName = "password_icon_" + DateTime.Now.Ticks + file.FileType;

                    StorageFolder applicationFolder = ApplicationData.Current.LocalFolder;
                    StorageFolder folder = await applicationFolder.CreateFolderAsync("Pic", CreationCollisionOption.OpenIfExists);

                    try
                    {
                        StorageFile saveFile = await file.CopyAsync(folder, desiredName, NameCollisionOption.GenerateUniqueName);
                        try
                        {
                            if (File.Exists(AddingItem.sPicture) &&
                                AddingItem.sPicture != saveFile.Path &&
                                !AddingItem.sPicture.Contains("Assets") &&
                                !AddingItem.sPicture.Contains("ms-appx"))
                            {
                                File.Delete(AddingItem.sPicture);
                            }
                        }
                        catch { }
                        AddingItem.sPicture = saveFile.Path;
                    }
                    catch
                    {
                        ShowNoFileToastGrid.Begin();
                        return;
                    }
                }
            }
            catch
            {
                try
                {
                    ShowNoFileToastGrid.Begin();
                }
                catch { }
            }
        }

        private void OnCompleteShowEmptyToast(object sender, object e)
        {
            try
            {
                HideEmptyToastGrid.Begin();
            }
            catch { }
        }

        private void OnCompleteShowNoFileToast(object sender, object e)
        {
            try
            {
                HideNoFileToastGrid.Begin();
            }
            catch { }
        }
    }
}
