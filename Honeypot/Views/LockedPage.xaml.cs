using System;
using System.Threading.Tasks;
using Honeypot.Helpers;
using Honeypot.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Honeypot.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class LockedPage : Page
{
    private bool _isUnlocking;

    public LockedPage()
    {
        InitializeComponent();
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        bool result = await UnlockAppAsync();
        if (result)
        {
            this.Frame.Navigate(typeof(MainPage));
            this.Frame.BackStack.Clear();
        }
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        bool result = await UnlockAppAsync();
        if (result)
        {
            this.Frame.Navigate(typeof(MainPage));
            this.Frame.BackStack.Clear();
        }
    }

    private async Task<bool> UnlockAppAsync()
    {
        try
        {
            if (_isUnlocking)
            {
                return false;
            }

            _isUnlocking = true;

            if (!App.Settings.LockWithWindowsHello)
            {
                return true;
            }

            switch (await Windows.Security.Credentials.UI.UserConsentVerifier.RequestVerificationAsync("UnlockAppUnlockingMessage".GetLocalized()))
            {
                case Windows.Security.Credentials.UI.UserConsentVerificationResult.Verified:
                    return true;
                case Windows.Security.Credentials.UI.UserConsentVerificationResult.DeviceNotPresent:
                case Windows.Security.Credentials.UI.UserConsentVerificationResult.NotConfiguredForUser:
                case Windows.Security.Credentials.UI.UserConsentVerificationResult.DisabledByPolicy:
                    await ContentDialogService.ShowAsync("UnlockAppUnlockFailed".GetLocalized(), "UnlockAppDeviceUnavailable".GetLocalized());
                    return false;
                case Windows.Security.Credentials.UI.UserConsentVerificationResult.DeviceBusy:
                    await ContentDialogService.ShowAsync("UnlockAppUnlockFailed".GetLocalized(), "UnlockAppDeviceBusy".GetLocalized());
                    return false;
                case Windows.Security.Credentials.UI.UserConsentVerificationResult.RetriesExhausted:
                    await ContentDialogService.ShowAsync("UnlockAppUnlockFailed".GetLocalized(), "UnlockAppRetriesExhausted".GetLocalized());
                    return false;
                case Windows.Security.Credentials.UI.UserConsentVerificationResult.Canceled:
                    return false;
                default:
                    return false;
            }
        }
        catch (Exception ex)
        {
            await ContentDialogService.ShowAsync("UnlockAppError".GetLocalized(), ex.Message);
            return false;
        }
        finally
        {
            _isUnlocking = false;
        }
    }
}
