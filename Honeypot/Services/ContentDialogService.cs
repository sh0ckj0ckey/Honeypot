using System;
using System.Threading;
using System.Threading.Tasks;
using Honeypot.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Honeypot.Services;

public static class ContentDialogService
{
    private static readonly SemaphoreSlim _contentDialogLock = new(1, 1);

    private static XamlRoot? _xamlRoot;
    private static Func<ElementTheme>? _themeProvider;

    /// <summary>
    /// Initializes the content dialog service with the current window XAML root.
    /// </summary>
    /// <param name="xamlRoot">The XAML root used to show content dialogs.</param>
    /// <param name="themeProvider">A function that returns the current app theme.</param>
    public static void Initialize(XamlRoot xamlRoot, Func<ElementTheme>? themeProvider = null)
    {
        _xamlRoot = xamlRoot ?? throw new ArgumentNullException(nameof(xamlRoot));
        _themeProvider = themeProvider;
    }

    /// <summary>
    /// Clears the current window context from the content dialog service.
    /// </summary>
    public static void Uninitialize()
    {
        _xamlRoot = null;
        _themeProvider = null;
    }

    /// <summary>
    /// Shows a content dialog.
    /// </summary>
    /// <param name="title">The dialog title.</param>
    /// <param name="content">The dialog content.</param>
    /// <param name="primaryButtonText">The primary button text.</param>
    /// <param name="secondaryButtonText">The secondary button text.</param>
    /// <param name="closeButtonText">The close button text. If all button texts are null or empty, the default localized close text is used.</param>
    /// <returns>The dialog result.</returns>
    public static async Task<ContentDialogResult> ShowAsync(object title, object content, string? primaryButtonText = null, string? secondaryButtonText = null, string? closeButtonText = null)
    {
        if (_xamlRoot is null)
        {
            throw new InvalidOperationException("ContentDialogService is not initialized.");
        }

        await _contentDialogLock.WaitAsync();

        try
        {
            bool hasPrimaryButton = !string.IsNullOrWhiteSpace(primaryButtonText);
            bool hasSecondaryButton = !string.IsNullOrWhiteSpace(secondaryButtonText);
            bool hasCloseButton = !string.IsNullOrWhiteSpace(closeButtonText);

            if (!hasPrimaryButton && !hasSecondaryButton && !hasCloseButton)
            {
                closeButtonText = "DialogButtonGotIt".GetLocalized();
            }

            ContentDialog contentDialog = new()
            {
                XamlRoot = _xamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                RequestedTheme = _themeProvider?.Invoke() ?? ElementTheme.Default,
                Title = title,
                Content = content,
                PrimaryButtonText = primaryButtonText ?? string.Empty,
                SecondaryButtonText = secondaryButtonText ?? string.Empty,
                CloseButtonText = closeButtonText ?? string.Empty
            };

            return await contentDialog.ShowAsync();
        }
        finally
        {
            _contentDialogLock.Release();
        }
    }
}
