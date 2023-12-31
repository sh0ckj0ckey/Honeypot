using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Honeypot.Helpers
{
    public static class LogoImageHelper
    {
        private static StorageFolder _imagesFolder = null;

        private static BitmapImage _defaultLogoBitmapImage = null;

        private static readonly Dictionary<string, BitmapImage> _logoImages = new Dictionary<string, BitmapImage>();

        public static async Task<StorageFolder> GetImagesFolder()
        {
            if (_imagesFolder is null)
            {
                string documentsFolderPath = UserDataPaths.GetDefault().Documents;
                StorageFolder documentsFolder = await StorageFolder.GetFolderFromPathAsync(documentsFolderPath);
                var noMewingFolder = await documentsFolder.CreateFolderAsync("NoMewing", CreationCollisionOption.OpenIfExists);
                var honeypotFolder = await noMewingFolder.CreateFolderAsync("Honeypot", CreationCollisionOption.OpenIfExists);
                var imagesFolder = await honeypotFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
                _imagesFolder = imagesFolder;
            }
            return _imagesFolder;
        }

        /// <summary>
        /// 根据文件名获取图片
        /// </summary>
        /// <param name="logoFileName"></param>
        /// <returns></returns>
        public static async Task<BitmapImage> GetLogoImage(string logoFileName, int width = 192)
        {
            try
            {
                if (!_logoImages.ContainsKey(logoFileName))
                {
                    var imageFolder = await GetImagesFolder();
                    string imageFilePath = Path.Combine(imageFolder.Path, logoFileName);
                    if (File.Exists(imageFilePath))
                    {
                        BitmapImage bitmapImage = new BitmapImage
                        {
                            DecodePixelType = DecodePixelType.Logical,
                            DecodePixelWidth = width,
                            UriSource = new Uri(imageFilePath)
                        };

                        _logoImages[logoFileName] = bitmapImage;
                    }
                }

                if (_logoImages.TryGetValue(logoFileName, out BitmapImage image))
                {
                    return image;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            try
            {
                _defaultLogoBitmapImage ??= new BitmapImage
                {
                    DecodePixelType = DecodePixelType.Logical,
                    DecodePixelWidth = 192,
                    UriSource = new Uri("ms-appx:///Assets/Icon/DefaultLogo.png")
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return _defaultLogoBitmapImage;
        }

        /// <summary>
        /// 将图片保存到文件
        /// </summary>
        /// <param name="logoFileName"></param>
        /// <param name="logoImage"></param>
        /// <returns></returns>
        public static async Task<bool> SaveLogoImage(string logoFileName, WriteableBitmap logoImage)
        {
            try
            {
                var imageFolder = await GetImagesFolder();
                var storageFile = await imageFolder.CreateFileAsync(logoFileName, CreationCollisionOption.ReplaceExisting);

                int retryAttempts = 3;
                const int ERROR_ACCESS_DENIED = unchecked((int)0x80070005);
                const int ERROR_SHARING_VIOLATION = unchecked((int)0x80070020);
                while (retryAttempts > 0)
                {
                    try
                    {
                        retryAttempts--;
                        using (IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            using Stream sourceStream = logoImage.PixelBuffer.AsStream();
                            var buffer = new byte[sourceStream.Length];
                            await sourceStream.ReadAsync(buffer);

                            BitmapEncoder bitmapEncoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                            bitmapEncoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)logoImage.PixelWidth, (uint)logoImage.PixelHeight, 96.0, 96.0, buffer);
                            await bitmapEncoder.FlushAsync();
                        }

                        return true;
                    }
                    catch (Exception ex) when ((ex.HResult == ERROR_ACCESS_DENIED) || (ex.HResult == ERROR_SHARING_VIOLATION))
                    {
                        await Task.Delay(TimeSpan.FromSeconds(2));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        await Task.Delay(TimeSpan.FromSeconds(2));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return false;
        }

        /// <summary>
        /// 将指定文件名的图片删除
        /// </summary>
        /// <param name="logoFileName"></param>
        public static async void DeleteLogoImage(string logoFileName)
        {
            try
            {
                _logoImages.Remove(logoFileName);
                var imageFolder = await GetImagesFolder();
                string imageFilePath = Path.Combine(imageFolder.Path, logoFileName);
                if (File.Exists(imageFilePath))
                {
                    var storageFile = await imageFolder.GetFileAsync(logoFileName);
                    await storageFile.DeleteAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
