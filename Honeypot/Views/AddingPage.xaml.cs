using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using Honeypot.Controls;
using Honeypot.Helpers;
using Honeypot.Models;
using Honeypot.ViewModels;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

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

        private ContentDialog _textEmptyDialog = null;

        private ContentDialog _setImageContentDialog = null;

        private WriteableBitmap _defaultLogoWriteableBitmap = null;
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

            _textEmptyDialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                CloseButtonText = "我知道了",
                DefaultButton = ContentDialogButton.Close
            };

            InitWriteableBitmaps();
        }

        /// <summary>
        /// 初始化默认图像
        /// </summary>
        private async void InitWriteableBitmaps()
        {
            try
            {
                if (_defaultLogoWriteableBitmap is null)
                {
                    var defaultLogoImage = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Icon/DefaultLogo.png"));
                    WriteableBitmap defaultImage = null;

                    using (IRandomAccessStream stream = await defaultLogoImage.OpenAsync(FileAccessMode.Read))
                    {
                        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                        defaultImage = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                        defaultImage.SetSource(stream);
                    }

                    _croppedWriteableBitmap = _defaultLogoWriteableBitmap = defaultImage;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 重置页面
        /// </summary>
        private void ResetPage()
        {
            try
            {
                _croppedWriteableBitmap = _defaultLogoWriteableBitmap;
                PreviewImageBursh.ImageSource = _croppedWriteableBitmap;

                AddingNameTextBox.Text = "";
                AddingAccountTextBox.Text = "";
                AddingPasswordTextBox.Text = "";
                AddingWebsiteTextBox.Text = "";
                AddingNoteTextBox.Text = "";
                AddingCategoryComboBox.SelectedIndex = -1;
                AddingFavoriteCheckBox.IsChecked = false;
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
        private async void OnClickChangeImage(object sender, RoutedEventArgs e)
        {
            try
            {
                _cropImageControl.SetOriginImage(_croppedWriteableBitmap);
                _setImageContentDialog.XamlRoot = this.XamlRoot;
                _setImageContentDialog.RequestedTheme = this.ActualTheme;
                ContentDialogResult result = await _setImageContentDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    var croppedImage = await _cropImageControl.GetCroppedImage();
                    _croppedWriteableBitmap = croppedImage;
                    PreviewImageBursh.ImageSource = _croppedWriteableBitmap;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 确认添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickConfirmAdd(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(AddingNameTextBox.Text))
                {
                    _textEmptyDialog.Title = "还不够...";
                    _textEmptyDialog.Content = "名称不能为空哦，再完善一下吧~";
                    _textEmptyDialog.XamlRoot = this.XamlRoot;
                    _textEmptyDialog.RequestedTheme = this.ActualTheme;
                    await _textEmptyDialog.ShowAsync();
                    return;
                }

                if (string.IsNullOrWhiteSpace(AddingAccountTextBox.Text) && string.IsNullOrWhiteSpace(AddingPasswordTextBox.Text))
                {
                    _textEmptyDialog.Title = "还不够...";
                    _textEmptyDialog.Content = "账号和密码至少要填写一个哦，再完善一下吧~";
                    _textEmptyDialog.XamlRoot = this.XamlRoot;
                    _textEmptyDialog.RequestedTheme = this.ActualTheme;
                    await _textEmptyDialog.ShowAsync();
                    return;
                }

                var name = AddingNameTextBox.Text;
                var account = AddingAccountTextBox.Text;
                var password = AddingPasswordTextBox.Text;
                var website = AddingWebsiteTextBox.Text;
                var note = AddingNoteTextBox.Text;
                var category = (AddingCategoryComboBox.SelectedItem as CategoryModel)?.Id ?? -1;
                var favorite = AddingFavoriteCheckBox.IsChecked == true;

                string logoFilePath = DateTime.Now.Ticks.ToString();
                bool result = await LogoImageHelper.SaveLogoImage(logoFilePath, _croppedWriteableBitmap);

                MainViewModel.Instance.AddPassword(category, account, password, name, website, note, favorite, logoFilePath);

                AddingSuccessBorder.Visibility = Visibility.Visible;
                await Task.Delay(600);
                ResetPage();
                AddingSuccessBorder.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                MainViewModel.Instance.ShowTipsContentDialog("糟糕...", $"添加密码时出现了异常：{ex.Message}");
            }
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
