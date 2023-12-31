namespace Honeypot.ViewModel
{
    public partial class PasswordViewModel : ViewModelBase
    {


        // 搜索
        public void SearchSuggestPasswords(string search)
        {
            try
            {
                vSearchSuggestions?.Clear();
                vSearchSuggestions = null;
                if (string.IsNullOrEmpty(search))
                {
                    return;
                }
                vSearchSuggestions = vAllPasswords.Where(p => (p.sName.StartsWith(search, StringComparison.CurrentCultureIgnoreCase) || p.sAccount.StartsWith(search, StringComparison.CurrentCultureIgnoreCase))).ToList();
            }
            catch { }
        }

        // 解锁应用程序
        public async void UnlockApp()
        {
            try
            {
                if (this.bLockEnabled == false)
                {
                    this.bAppLocked = false;
                    return;
                }

                switch (await Windows.Security.Credentials.UI.UserConsentVerifier.RequestVerificationAsync("验证您的身份"))
                {
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.Verified:
                        this.bAppLocked = false;
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.DeviceNotPresent:
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.NotConfiguredForUser:
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.DisabledByPolicy:
                        await new Windows.UI.Popups.MessageDialog("当前识别设备未配置或被系统策略禁用，请尝试使用密码解锁").ShowAsync();
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.DeviceBusy:
                        await new Windows.UI.Popups.MessageDialog("当前识别设备不可用，请尝试使用密码解锁").ShowAsync();
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.RetriesExhausted:
                        await new Windows.UI.Popups.MessageDialog("验证失败，请尝试使用密码解锁").ShowAsync();
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.Canceled:
                        break;
                    default:
                        break;
                }
            }
            catch { }
        }

        // 开关WindowsHello
        public async void SetWindowsHelloEnable(bool on)
        {
            try
            {
                if (this.bLockEnabled != on)
                {
                    this.bLockEnabled = !this.bLockEnabled;
                    this.bLockEnabled = !this.bLockEnabled;
                    switch (await Windows.Security.Credentials.UI.UserConsentVerifier.RequestVerificationAsync("验证您的身份"))
                    {
                        case Windows.Security.Credentials.UI.UserConsentVerificationResult.Verified:
                            this.bLockEnabled = on;
                            App.AppSettingContainer.Values["bAppLockEnabled"] = this.bLockEnabled ? "True" : "False";
                            break;
                        case Windows.Security.Credentials.UI.UserConsentVerificationResult.DeviceNotPresent:
                        case Windows.Security.Credentials.UI.UserConsentVerificationResult.NotConfiguredForUser:
                        case Windows.Security.Credentials.UI.UserConsentVerificationResult.DisabledByPolicy:
                            await new Windows.UI.Popups.MessageDialog("当前识别设备未配置或被系统策略禁用").ShowAsync();
                            break;
                        case Windows.Security.Credentials.UI.UserConsentVerificationResult.DeviceBusy:
                            await new Windows.UI.Popups.MessageDialog("当前识别设备不可用").ShowAsync();
                            break;
                        case Windows.Security.Credentials.UI.UserConsentVerificationResult.RetriesExhausted:
                            await new Windows.UI.Popups.MessageDialog("验证失败").ShowAsync();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch { }
        }

    }
}
