using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Honeypot.Services;

/// <summary>
/// Represents the available decoded logo image sizes.
/// </summary>
public enum LogoImageSize
{
    Small,
    Medium,
    Large
}

/// <summary>
/// Provides file and cache operations for account logo images.
/// </summary>
public static class LogoService
{
    private const string DefaultLogoUri = "ms-appx:///Assets/Icon/img_default.png";
    private const string DefaultLogoCacheKey = "__default_logo__";

    private static readonly Dictionary<LogoImageSize, Dictionary<string, BitmapImage>> _logoImageCache = [];

    private static StorageFolder? _imagesFolder;

    private static int GetDecodePixelWidth(LogoImageSize size) => size switch
    {
        LogoImageSize.Small => 48,
        LogoImageSize.Medium => 96,
        LogoImageSize.Large => 192,
        _ => 96
    };

    /// <summary>
    /// Gets the folder used to store logo images.
    /// </summary>
    /// <returns>The images folder.</returns>
    private static async Task<StorageFolder> GetImagesFolderAsync()
    {
        if (_imagesFolder is not null)
        {
            return _imagesFolder;
        }

        string documentsFolderPath = UserDataPaths.GetDefault().Documents;
        StorageFolder documentsFolder = await StorageFolder.GetFolderFromPathAsync(documentsFolderPath);
        StorageFolder noMewingFolder = await documentsFolder.CreateFolderAsync("NoMewing", CreationCollisionOption.OpenIfExists);
        StorageFolder honeypotFolder = await noMewingFolder.CreateFolderAsync("Honeypot", CreationCollisionOption.OpenIfExists);
        _imagesFolder = await honeypotFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);

        return _imagesFolder;
    }

    /// <summary>
    /// Gets the stored logo image file.
    /// For compatibility, <paramref name="logoFileName"/> may be either a file name or a full path.
    /// When a full path is provided, only the file name part is used.
    /// </summary>
    /// <param name="logoFileName">The logo file name, or a legacy full path.</param>
    /// <returns>The storage file if it exists; otherwise, <see langword="null"/>.</returns>
    public static async Task<StorageFile?> GetImageFileAsync(string? logoFileName)
    {
        if (string.IsNullOrWhiteSpace(logoFileName))
        {
            return null;
        }

        string fileName = System.IO.Path.GetFileName(logoFileName.Trim());

        if (string.IsNullOrWhiteSpace(fileName))
        {
            return null;
        }

        try
        {
            StorageFolder imagesFolder = await GetImagesFolderAsync();
            IStorageItem? item = await imagesFolder.TryGetItemAsync(fileName);
            return item as StorageFile;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine(ex);
            return null;
        }
    }

    /// <summary>
    /// Gets a logo image for the specified file name and decode size.
    /// For compatibility, <paramref name="logoFileName"/> may be either a file name or a full path.
    /// When a full path is provided, only the file name part is used.
    /// Returns the default logo image when the value is invalid, the file does not exist, or the image cannot be loaded.
    /// </summary>
    /// <param name="logoFileName">The logo file name, or a legacy full path.</param>
    /// <param name="size">The requested decode size.</param>
    /// <returns>A bitmap image.</returns>
    public static async Task<BitmapImage> GetLogoImageAsync(string? logoFileName, LogoImageSize size)
    {
        if (!_logoImageCache.TryGetValue(size, out Dictionary<string, BitmapImage>? sizeCache))
        {
            sizeCache = [];
            _logoImageCache[size] = sizeCache;
        }

        string? fileName = System.IO.Path.GetFileName(logoFileName?.Trim());

        if (!string.IsNullOrWhiteSpace(fileName))
        {
            if (sizeCache.TryGetValue(fileName, out BitmapImage? cachedImage))
            {
                return cachedImage;
            }

            try
            {
                StorageFile? imageFile = await GetImageFileAsync(fileName);

                if (imageFile is not null)
                {
                    BitmapImage bitmapImage = new()
                    {
                        DecodePixelType = DecodePixelType.Logical,
                        DecodePixelWidth = GetDecodePixelWidth(size)
                    };

                    using IRandomAccessStream stream = await imageFile.OpenReadAsync();
                    await bitmapImage.SetSourceAsync(stream);

                    sizeCache[fileName] = bitmapImage;
                    return bitmapImage;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
        }

        if (sizeCache.TryGetValue(DefaultLogoCacheKey, out BitmapImage? defaultCachedImage))
        {
            return defaultCachedImage;
        }

        BitmapImage defaultImage = new()
        {
            DecodePixelType = DecodePixelType.Logical,
            DecodePixelWidth = GetDecodePixelWidth(size),
            UriSource = new Uri(DefaultLogoUri, UriKind.Absolute)
        };

        sizeCache[DefaultLogoCacheKey] = defaultImage;
        return defaultImage;
    }

    /// <summary>
    /// Saves a logo image as a PNG file with a generated unique file name.
    /// </summary>
    /// <param name="logoImage">The source logo image.</param>
    /// <returns>
    /// The generated logo file name if the image was saved successfully; otherwise, <see langword="null"/>.
    /// </returns>
    public static async Task<string?> SaveLogoImageAsync(WriteableBitmap logoImage)
    {
        if (logoImage is null || logoImage.PixelWidth <= 0 || logoImage.PixelHeight <= 0)
        {
            return null;
        }

        const int MaxRetryAttempts = 3;
        const int ErrorAccessDenied = unchecked((int)0x80070005);
        const int ErrorSharingViolation = unchecked((int)0x80070020);
        const int ErrorLockViolation = unchecked((int)0x80070021);

        StorageFile? storageFile = null;

        try
        {
            byte[] pixels = logoImage.PixelBuffer.ToArray();

            if (pixels.Length == 0)
            {
                return null;
            }

            StorageFolder imagesFolder = await GetImagesFolderAsync();

            string suggestedFileName = $"{Guid.NewGuid():N}.png";
            storageFile = await imagesFolder.CreateFileAsync(suggestedFileName, CreationCollisionOption.GenerateUniqueName);

            for (int attempt = 1; attempt <= MaxRetryAttempts; attempt++)
            {
                try
                {
                    using IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.ReadWrite);
                    stream.Size = 0;

                    BitmapEncoder bitmapEncoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                    bitmapEncoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)logoImage.PixelWidth, (uint)logoImage.PixelHeight, 96.0, 96.0, pixels);

                    await bitmapEncoder.FlushAsync();

                    return storageFile.Name;
                }
                catch (Exception ex) when (attempt < MaxRetryAttempts && (ex.HResult == ErrorAccessDenied || ex.HResult == ErrorSharingViolation || ex.HResult == ErrorLockViolation))
                {
                    System.Diagnostics.Trace.WriteLine(ex);
                    await Task.Delay(TimeSpan.FromMilliseconds(200 * attempt));
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine(ex);
        }

        if (storageFile is not null)
        {
            try
            {
                await storageFile.DeleteAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
        }

        return null;
    }

    /// <summary>
    /// Deletes a stored logo image file and removes it from the in-memory cache.
    /// For compatibility, <paramref name="logoFileName"/> may be either a file name or a full path.
    /// When a full path is provided, only the file name part is used.
    /// </summary>
    /// <param name="logoFileName">The logo file name, or a legacy full path.</param>
    /// <returns>
    /// <see langword="true"/> if the file was deleted, does not exist, or the value is invalid; otherwise, <see langword="false"/>.
    /// </returns>
    public static async Task<bool> DeleteLogoImageAsync(string? logoFileName)
    {
        if (string.IsNullOrWhiteSpace(logoFileName))
        {
            return true;
        }

        string fileName = System.IO.Path.GetFileName(logoFileName.Trim());

        if (string.IsNullOrWhiteSpace(fileName))
        {
            return true;
        }

        foreach (Dictionary<string, BitmapImage> sizeCache in _logoImageCache.Values)
        {
            sizeCache.Remove(fileName);
        }

        try
        {
            StorageFile? imageFile = await GetImageFileAsync(fileName);

            if (imageFile is null)
            {
                return true;
            }

            await imageFile.DeleteAsync();

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine(ex);
            return false;
        }
    }
}
