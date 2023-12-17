using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Composition;
using System.Numerics;
using Honeypot.Controls;
using Honeypot.ViewModels;
using Honeypot.Models;
using Windows.Storage;
using System.Diagnostics;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Honeypot.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddingPage : Page
    {
        private MainViewModel MainViewModel = null;

        private CropImageControl _cropImageControl = new CropImageControl();

        private ContentDialog _setImageContentDialog = null;

        private WriteableBitmap _defaultAvatarWriteableBitmap = null;
        private WriteableBitmap _croppedWriteableBitmap = null;

        public AddingPage()
        {
            this.InitializeComponent();

            MainViewModel = MainViewModel.Instance;

            _setImageContentDialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "裁剪图像",
                Content = _cropImageControl,
                PrimaryButtonText = "确定",
                CloseButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary
            };

            InitWriteableBitmaps();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _setImageContentDialog.PrimaryButtonClick += OnDialogClickConfirm;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _setImageContentDialog.PrimaryButtonClick -= OnDialogClickConfirm;
        }

        private async void InitWriteableBitmaps()
        {
            try
            {
                if (_defaultAvatarWriteableBitmap is null)
                {
                    var defaulAvatartImage = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Icon/DefaultAvatar.png"));
                    WriteableBitmap defaultImage = null;
                    using (IRandomAccessStream stream = await defaulAvatartImage.OpenAsync(FileAccessMode.Read))
                    {
                        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                        defaultImage = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                        defaultImage.SetSource(stream);
                    }
                    _croppedWriteableBitmap = _defaultAvatarWriteableBitmap = defaultImage;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 点击弹出对话框，裁剪头像图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickSetImage(object sender, RoutedEventArgs e)
        {
            try
            {
                _cropImageControl.InitializeImage(_croppedWriteableBitmap);
                _setImageContentDialog.XamlRoot = this.XamlRoot;
                _setImageContentDialog.RequestedTheme = this.ActualTheme;
                await _setImageContentDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 对话框点击确认，生成头像图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void OnDialogClickConfirm(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var croppedImage = await _cropImageControl.GetCroppedImage();
            _croppedWriteableBitmap = croppedImage;
            PreviewImageBursh.ImageSource = _croppedWriteableBitmap;
        }

        #region ChangeImageButton animation

        private readonly Compositor _compositor = App.MainWindow.Compositor;
        private SpringVector3NaturalMotionAnimation _springAnimation;

        private void CreateOrUpdateSpringAnimation(float finalValue)
        {
            try
            {
                if (_springAnimation == null)
                {
                    _springAnimation = _compositor.CreateSpringVector3Animation();
                    _springAnimation.Target = "Scale";
                    _springAnimation.DampingRatio = 0.5f;
                }

                _springAnimation.FinalValue = new Vector3(finalValue);
            }
            catch { }
        }

        private void SpringAnimationPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                CreateOrUpdateSpringAnimation(1.1f);
                (sender as UIElement).StartAnimation(_springAnimation);
            }
            catch { }
        }

        private void SpringAnimationPointerExited(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                CreateOrUpdateSpringAnimation(1.0f);
                (sender as UIElement).StartAnimation(_springAnimation);
            }
            catch { }
        }

        #endregion

    }
}
