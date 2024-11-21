using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Honeypot.Core;
using Honeypot.Data;
using Honeypot.Helpers;
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

        /// <summary>
        /// 控制MainFrame导航页面
        /// </summary>
        public Action<NavigatePageEnum> ActNavigatePage { get; set; } = null;

        /// <summary>
        /// 在密码列表页显示一个提示，提醒用户定期备份数据
        /// </summary>
        public Action<int> ActNoticeUserToBackup { get; set; } = null;

        /// <summary>
        /// 应用程序是否被锁定
        /// </summary>
        private bool _isLocked = true;
        public bool IsLocked
        {
            get => _isLocked;
            set => SetProperty(ref _isLocked, value);
        }

        public ObservableCollection<MainNavigationBase> MainNavigationItems = new ObservableCollection<MainNavigationBase>();

        public ObservableCollection<MainNavigationBase> MainNavigationFooterItems = new ObservableCollection<MainNavigationBase>();

        public MainViewModel()
        {
            AppSettings.OnAppearanceSettingChanged += (index) => { ActSwitchAppTheme?.Invoke(); };
            AppSettings.OnBackdropSettingChanged += (index) => { ActChangeBackdrop?.Invoke(); };

            var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();

            MainNavigationItems.Add(new MainNavigationItem(HoneypotConsts.AllPasswordsPageTitle, "passwords", "\uE8D7"));
            MainNavigationItems.Add(new MainNavigationItem(resourceLoader.GetString("NavigationItemFavorites"), "favorites", "\uEB51"));
            MainNavigationItems.Add(new MainNavigationItem(resourceLoader.GetString("NavigationItemAdd"), "adding", "\uE109"));
            MainNavigationItems.Add(new MainNavigationSeparator());
            MainNavigationItems.Add(new MainNavigationItem(resourceLoader.GetString("NavigationItemCategories"), "category", "\uE74C", CategoriesOnNav));
            MainNavigationFooterItems.Add(new MainNavigationItem(resourceLoader.GetString("NavigationItemGenerator"), "random", "\uF439"));
            MainNavigationFooterItems.Add(new MainNavigationSeparator());
            MainNavigationFooterItems.Add(new MainNavigationItem(resourceLoader.GetString("NavigationItemTips"), "tips", "\uE82F"));
            MainNavigationFooterItems.Add(new MainNavigationSettingItem(resourceLoader.GetString("NavigationItemSettings")));

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

                var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
                ShowTipsContentDialog(resourceLoader.GetString("DialogTitleOops"), $"{resourceLoader.GetString("DialogContentWrongConnectToDb")}: {ex.Message}");
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

        /// <summary>
        /// 锁定应用程序
        /// </summary>
        public void LockApp()
        {
            if (AppSettings.EnableLock == true)
            {
                IsLocked = true;
            }
        }

        /// <summary>
        /// 解锁应用程序
        /// </summary>
        public async void UnlockApp()
        {
            try
            {
                if (AppSettings.EnableLock == false)
                {
                    IsLocked = false;
                    return;
                }

                var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();

                switch (await Windows.Security.Credentials.UI.UserConsentVerifier.RequestVerificationAsync(resourceLoader.GetString("UnlockAppUnlockingMessage")))
                {
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.Verified:
                        IsLocked = false;
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.DeviceNotPresent:
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.NotConfiguredForUser:
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.DisabledByPolicy:
                        ShowTipsContentDialog(resourceLoader.GetString("UnlockAppUnlockFailed"), resourceLoader.GetString("UnlockAppDeviceUnavailable"));
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.DeviceBusy:
                        ShowTipsContentDialog(resourceLoader.GetString("UnlockAppUnlockFailed"), resourceLoader.GetString("UnlockAppDeviceBusy"));
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.RetriesExhausted:
                        ShowTipsContentDialog(resourceLoader.GetString("UnlockAppUnlockFailed"), resourceLoader.GetString("UnlockAppRetriesExhausted"));
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.Canceled:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
                ShowTipsContentDialog(resourceLoader.GetString("UnlockAppError"), $"{ex.Message}");
            }
        }

    }
}
