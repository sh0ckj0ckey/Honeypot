using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Honeypot.Data.Models;
using Honeypot.Services;
using Windows.Storage;

namespace Honeypot.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly Dictionary<long, PasswordItemViewModel> _passwordsById = [];

    private readonly Dictionary<long, CategoryItemViewModel> _categoriesById = [];

    private bool _isLoaded;

    private bool _isLoading;

    private CategoryItemViewModel? _selectedCategory;

    private PasswordItemViewModel? _selectedPassword;

    public NavigationViewModel Navigation { get; } = new();

    /// <summary>
    /// Gets all category items loaded from the database.
    /// </summary>
    public ObservableCollection<CategoryItemViewModel> AllCategories { get; } = [];

    /// <summary>
    /// Gets all password items loaded from the database.
    /// This collection is also used as the source for third-party login selection.
    /// </summary>
    public ObservableCollection<PasswordItemViewModel> AllPasswords { get; } = [];

    /// <summary>
    /// Gets favorite passwords grouped by category.
    /// </summary>
    public ObservableCollection<PasswordGroupViewModel> FavoritePasswordGroups { get; } = [];

    /// <summary>
    /// Gets the current filtered password list sorted by added time.
    /// </summary>
    public ObservableCollection<PasswordItemViewModel> CurrentPasswords { get; } = [];

    /// <summary>
    /// Gets the current filtered password list grouped by first letter.
    /// </summary>
    public ObservableCollection<PasswordGroupViewModel> CurrentPasswordGroups { get; } = [];

    /// <summary>
    /// Gets or sets whether the main view model is currently loading data.
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public CategoryItemViewModel? SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    public PasswordItemViewModel? SelectedPassword
    {
        get => _selectedPassword;
        set => SetProperty(ref _selectedPassword, value);
    }

    public async Task LoadAsync()
    {
        if (_isLoaded || this.IsLoading)
        {
            return;
        }

        try
        {
            this.IsLoading = true;

            List<CategoryItemViewModel> categories = [];
            List<PasswordItemViewModel> passwords = [];

            await Task.Run(async () =>
            {
                await PasswordsService.InitializeAsync();

                var categoryModels = PasswordsService.GetCategories();
                var passwordModels = PasswordsService.GetPasswords();

                foreach (var categoryModel in categoryModels)
                {
                    if (categoryModel is null)
                    {
                        continue;
                    }

                    categories.Add(new CategoryItemViewModel(categoryModel));
                }

                foreach (var passwordModel in passwordModels)
                {
                    if (passwordModel is null)
                    {
                        continue;
                    }

                    passwords.Add(new PasswordItemViewModel(passwordModel));
                }

                categories.Sort((x, y) => x.Order.CompareTo(y.Order));
                passwords.Sort((x, y) => x.Id.CompareTo(y.Id));
            });

            // Load categories into the view model
            foreach (var category in categories)
            {
                _categoriesById[category.Id] = category;
                this.AllCategories.Add(category);
            }

            // Load passwords into the view model
            foreach (var password in passwords)
            {
                _passwordsById[password.Id] = password;
                this.AllPasswords.Add(password);

                if (password.Favorite)
                {

                }
            }

            // Update the navigation view with the loaded categories
            this.Navigation.UpdateCategories(this.AllCategories);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine(ex.Message);
        }
        finally
        {
            this.IsLoading = false;
        }
    }

    public void Clear()
    {
        this.SelectedCategory = null;
        this.SelectedPassword = null;

        this.AllCategories.Clear();
        this.AllPasswords.Clear();
        this.CurrentPasswords.Clear();
        this.CurrentPasswordGroups.Clear();
        this.FavoritePasswordGroups.Clear();

        _passwordsById.Clear();
        _categoriesById.Clear();

        this.Navigation.UpdateCategories(this.AllCategories);

        _isLoaded = false;
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
            System.Diagnostics.Trace.WriteLine(ex.Message);

            var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
            ShowTipsContentDialog(resourceLoader.GetString("DialogTitleOops"), $"{resourceLoader.GetString("DialogContentWrongConnectToDb")}: {ex.Message}");
        }
    }

    public ReadOnlyCollection<PasswordModel> Search(string keyword)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var suggests = Passwords.Where(p => p.Name.StartsWith(keyword, StringComparison.CurrentCultureIgnoreCase) || p.Account.StartsWith(keyword, StringComparison.CurrentCultureIgnoreCase));
                return suggests;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine(ex.Message);
        }

        return [];
    }

    /// <summary>
    /// 更新收藏夹列表
    /// </summary>
    private void UpdateFavorites()
    {
        FavoritePasswordsGroups.Clear();

        // 收藏夹
        var orderedFavoriteList =
            (from item in AllPasswords
             where item.Favorite
             group item by item.CategoryId into newItems
             select
             new FavoritesGroupModel
             {
                 Key = newItems.Key,
                 Passwords = new ObservableCollection<PasswordModel>(newItems.ToList())
             }).OrderBy(x => x.Key).ToList();

        foreach (var item in orderedFavoriteList)
        {
            FavoritePasswordsGroups.Add(item);
        }
    }

    /// <summary>
    /// 收藏/取消收藏密码
    /// </summary>
    /// <param name="passwordItem"></param>
    public void FavoritePassword(PasswordModel passwordItem)
    {
        try
        {
            passwordItem.Favorite = !passwordItem.Favorite;
            PasswordsDataAccess.FavoritePassword(passwordItem.Id, passwordItem.Favorite);

            UpdateFavorites();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);

            var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
            ShowTipsContentDialog(resourceLoader.GetString("DialogTitleOops"), $"{resourceLoader.GetString("DialogContentWrongAddFavorite")}: {ex.Message}");
        }
    }
}
