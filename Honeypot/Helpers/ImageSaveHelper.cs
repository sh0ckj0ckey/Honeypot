using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Honeypot.Helpers
{
    public static class ImageSaveHelper
    {
        private static StorageFolder _imagesFolder = null;

        private static WriteableBitmap _defaultLogoWriteableBitmap = null;

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

        public static async Task<WriteableBitmap> GetLogoImage(string logoFileName)
        {
            try
            {
                var imageFolder = await GetImagesFolder();
                var storageFile = await imageFolder.GetFileAsync(logoFileName);
                WriteableBitmap image = null;
                using (IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.Read))
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                    image = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                    image.SetSource(stream);
                }

                if (image is not null)
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
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return _defaultLogoWriteableBitmap;
        }

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

        public static async void DeleteLogoImage(string filePath)
        {
            try
            {
                var imageFolder = await GetImagesFolder();
                var storageFile = await imageFolder.GetFileAsync(filePath);
                await storageFile.DeleteAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
