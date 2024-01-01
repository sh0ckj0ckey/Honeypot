using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Honeypot.Core;
using Honeypot.Data;
using Honeypot.Models;
using Windows.Storage;

namespace Honeypot.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private static Lazy<MainViewModel> _lazyVM = new Lazy<MainViewModel>(() => new MainViewModel());
        public static MainViewModel Instance => _lazyVM.Value;

        public SettingsService AppSettings { get; set; } = new SettingsService();

        /// <summary>
        /// 控制主窗口根据当前的主题进行切换
        /// </summary>
        public Action ActSwitchAppTheme { get; set; } = null;

        /// <summary>
        /// 控制主窗口根据当前的设置更改背景材质
        /// </summary>
        public Action ActChangeBackdrop { get; set; } = null;

        /// <summary>
        /// 弹出提示框
        /// </summary>
        public Action<string, string> ActShowTipDialog { get; set; } = null;


        public ObservableCollection<MainNavigationBase> MainNavigationItems = new ObservableCollection<MainNavigationBase>();

        public ObservableCollection<MainNavigationBase> MainNavigationFooterItems = new ObservableCollection<MainNavigationBase>();

        public MainViewModel()
        {
            // 注册设置变更事件
            AppSettings.OnAppearanceSettingChanged += (index) => { ActSwitchAppTheme?.Invoke(); };
            AppSettings.OnBackdropSettingChanged += (index) => { ActChangeBackdrop?.Invoke(); };

            // 导航栏
            MainNavigationItems.Add(new MainNavigationItem("所有账号", "all", "\uE8D7"));
            MainNavigationItems.Add(new MainNavigationItem("收藏夹", "favorite", "\uEB51"));
            MainNavigationItems.Add(new MainNavigationItem("添加", "adding", "\uE109"));
            MainNavigationItems.Add(new MainNavigationSeparator());
            MainNavigationItems.Add(new MainNavigationItem("全部分类", "category", "\uE74C", CategoriesOnNav));

            // 导航栏底部
            MainNavigationFooterItems.Add(new MainNavigationItem("密码生成器", "random", "\uF439"));
            MainNavigationFooterItems.Add(new MainNavigationSeparator());
            MainNavigationFooterItems.Add(new MainNavigationItem("使用提示", "tips", "\uE82F"));
            MainNavigationFooterItems.Add(new MainNavigationSettingItem());

            // 加载数据库
            InitPasswordsDataBase();
        }

        /// <summary>
        /// 加载数据库
        /// </summary>
        public async void InitPasswordsDataBase()
        {
            try
            {
                StorageFolder documentsFolder = await StorageFolder.GetFolderFromPathAsync(UserDataPaths.GetDefault().Documents);
                var noMewingFolder = await documentsFolder.CreateFolderAsync("NoMewing", CreationCollisionOption.OpenIfExists);
                var dbFolder = await noMewingFolder.CreateFolderAsync("Honeypot", CreationCollisionOption.OpenIfExists);
                PasswordsDataAccess.CloseDatabase();
                await PasswordsDataAccess.LoadDatabase(dbFolder);

                LoadPasswordsTable();
                LoadCategoriesTable();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                ShowTipsContentDialog("糟糕...", $"连接数据库时出现了异常：{ex.Message}");
            }
        }

        /// <summary>
        /// 弹出对话框提示用户特定内容
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        public void ShowTipsContentDialog(string title, string content)
        {
            try
            {
                ActShowTipDialog?.Invoke(title, content);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        // 解锁应用程序
        public async void UnlockApp()
        {
            try
            {
                if (AppSettings.EnableLock == false)
                {
                    Unlock();
                    return;
                }

                switch (await Windows.Security.Credentials.UI.UserConsentVerifier.RequestVerificationAsync("验证您的身份"))
                {
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.Verified:
                        Unlock();
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
                if (AppSettings.EnableLock != on)
                {
                    switch (await Windows.Security.Credentials.UI.UserConsentVerifier.RequestVerificationAsync("验证您的身份"))
                    {
                        case Windows.Security.Credentials.UI.UserConsentVerificationResult.Verified:
                            AppSettings.EnableLock = on;
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
