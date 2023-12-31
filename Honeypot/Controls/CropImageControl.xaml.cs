using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Honeypot.Controls
{
    public sealed partial class CropImageControl : UserControl
    {
        public CropImageControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 设置初始图片
        /// </summary>
        /// <param name="origImage"></param>
        public void SetOriginImage(WriteableBitmap origImage)
        {
            try
            {
                LogoImageCropper.Source = origImage;
                LogoImageCropper.Reset();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 获取裁剪最后得到的图片
        /// </summary>
        /// <returns></returns>
        public async Task<WriteableBitmap> GetCroppedImage()
        {
            try
            {
                using var inMemoryRandomStream = new InMemoryRandomAccessStream();
                await LogoImageCropper.SaveAsync(inMemoryRandomStream, BitmapFileFormat.Png);
                inMemoryRandomStream.Seek(0);
                var bitmap = new WriteableBitmap(1, 1);
                bitmap.SetSource(inMemoryRandomStream);
                return bitmap;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// 点击按钮选择新图片文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickPickFile(object sender, RoutedEventArgs e)
        {
            try
            {
                //FileTooLargeInfoBar.IsOpen = false;

                var filePicker = new FileOpenPicker
                {
                    ViewMode = PickerViewMode.Thumbnail,
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                    FileTypeFilter = { ".png", ".jpg", ".jpeg" }
                };

                WinRT.Interop.InitializeWithWindow.Initialize(filePicker, App.MainWindow.GetWindowHandle());

                var file = await filePicker.PickSingleFileAsync();
                if (file != null && LogoImageCropper != null)
                {
                    await LogoImageCropper.LoadImageFromFile(file);
                    //var property = await file.GetBasicPropertiesAsync();
                    //if ((property?.Size ?? ulong.MaxValue) > 8 * 1024 * 1024)
                    //{
                    //    FileTooLargeInfoBar.IsOpen = true;
                    //}
                    //else
                    //{
                    //    await LogoImageCropper.LoadImageFromFile(file);
                    //}
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
