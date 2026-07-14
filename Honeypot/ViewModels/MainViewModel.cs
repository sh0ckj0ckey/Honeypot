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
    /// 在密码列表页显示一个提示，提醒用户定期备份数据
    /// </summary>
    public Action<int> ActNoticeUserToBackup { get; set; } = null;

    public MainViewModel()
    {
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
