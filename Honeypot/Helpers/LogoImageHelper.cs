using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Honeypot.Models;
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

        private static readonly Dictionary<LogoSizeEnum, Dictionary<string, BitmapImage>> _logoImages = new();

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
        public static async Task<BitmapImage> GetLogoImage(string logoFileName, LogoSizeEnum size)
        {
            try
            {
                if (!_logoImages.ContainsKey(size))
                {
                    _logoImages[size] = new Dictionary<string, BitmapImage>();
                }

                if (!_logoImages[size].ContainsKey(logoFileName))
                {
                    var imageFolder = await GetImagesFolder();
                    string imageFilePath = Path.Combine(imageFolder.Path, logoFileName);
                    if (File.Exists(imageFilePath))
                    {
                        BitmapImage bitmapImage = new BitmapImage
                        {
                            DecodePixelType = DecodePixelType.Logical,
                            DecodePixelWidth = size == LogoSizeEnum.Large ? 192 : (size == LogoSizeEnum.Small ? 48 : 96),
                            UriSource = new Uri(imageFilePath)
                        };

                        _logoImages[size][logoFileName] = bitmapImage;
                    }
                }

                if (_logoImages[size].TryGetValue(logoFileName, out BitmapImage image))
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
                    UriSource = new Uri("ms-appx:///Assets/Icon/img_default.png")
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
                foreach (var size in _logoImages)
                {
                    size.Value.Remove(logoFileName);
                }

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
