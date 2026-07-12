using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Honeypot.Helpers;
using Honeypot.Models;
using Windows.Storage;

namespace Honeypot.ViewModels;

public partial class MainViewModel : ObservableObject
{
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
}
