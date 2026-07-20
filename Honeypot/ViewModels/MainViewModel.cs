using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Honeypot.Data.Models;
using Honeypot.Helpers;
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

    /// <summary>
    /// Gets the view model that provides navigation items for the main navigation view.
    /// </summary>
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
    /// Gets the current filtered password items sorted by added time.
    /// </summary>
    public ObservableCollection<PasswordItemViewModel> CurrentPasswordItems { get; } = [];

    /// <summary>
    /// Gets the current filtered password items grouped by first letter.
    /// </summary>
    public ObservableCollection<PasswordGroupViewModel> CurrentPasswordGroups { get; } = [];

    /// <summary>
    /// Gets favorite password items grouped by category.
    /// </summary>
    public ObservableCollection<PasswordGroupViewModel> FavoritePasswordGroups { get; } = [];

    /// <summary>
    /// Gets or sets whether the main view model is currently loading data.
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    /// <summary>
    /// Gets or sets the category currently used to filter the password list.
    /// A <see langword="null"/> value means all passwords are shown.
    /// </summary>
    public CategoryItemViewModel? SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    /// <summary>
    /// Gets or sets the password item currently selected in the password list.
    /// </summary>
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

            this.SelectedCategory = null;
            this.SelectedPassword = null;

            _categoriesById.Clear();
            _passwordsById.Clear();
            this.AllCategories.Clear();
            this.AllPasswords.Clear();

            List<CategoryItemViewModel> categories = [];
            List<PasswordItemViewModel> passwords = [];
            Dictionary<long, long> passwordToCategory = [];
            Dictionary<long, long> passwordToThirdParty = [];

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

                    if (passwordModel.CategoryId > 0)
                    {
                        passwordToCategory[passwordModel.Id] = passwordModel.CategoryId;
                    }
                    if (passwordModel.ThirdPartyId > 0)
                    {
                        passwordToThirdParty[passwordModel.Id] = passwordModel.ThirdPartyId;
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
            }

            // Update category and third-party references for each password item
            foreach (var password in passwords)
            {
                if (passwordToCategory.TryGetValue(password.Id, out var categoryId) && _categoriesById.TryGetValue(categoryId, out var category))
                {
                    password.Category = category;
                }
                if (passwordToThirdParty.TryGetValue(password.Id, out var thirdPartyId) && _passwordsById.TryGetValue(thirdPartyId, out var thirdParty))
                {
                    password.ThirdParty = thirdParty;
                }
            }

            _ = this.UpdateCurrentPasswordsAsync();
            _ = this.UpdateFavoritePasswordsAsync();

            // Update the navigation view with the loaded categories
            this.Navigation.UpdateCategories(this.AllCategories);

            _isLoaded = true;
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

        _passwordsById.Clear();
        _categoriesById.Clear();
        this.AllCategories.Clear();
        this.AllPasswords.Clear();
        this.CurrentPasswordItems.Clear();
        this.CurrentPasswordGroups.Clear();
        this.FavoritePasswordGroups.Clear();

        this.Navigation.UpdateCategories(this.AllCategories);

        _isLoaded = false;
    }

    private async Task UpdateCurrentPasswordsAsync()
    {
        this.CurrentPasswordItems.Clear();
        this.CurrentPasswordGroups.Clear();

        if (!_isLoaded)
        {
            return;
        }



        // Load logo images for current password items
        foreach (var password in this.CurrentPasswordItems)
        {
            password.LogoImage ??= await LogosService.GetLogoImageAsync(password.LogoImageFileName, LogoImageSize.Medium);
        }
    }

    private async Task UpdateFavoritePasswordsAsync()
    {
        this.FavoritePasswordGroups.Clear();

        if (!_isLoaded)
        {
            return;
        }

        var favoriteGroups = this.AllPasswords
            .Where(password => password.Favorite)
            .GroupBy(password => password.Category)
            .OrderBy(group => group.Key?.Order ?? long.MaxValue)
            .ThenBy(group => group.Key?.Title ?? string.Empty)
            .Select(group => new PasswordGroupViewModel(
                group.Key?.Title ?? "UncategorizedCategoryTitle".GetLocalized(),
                new ObservableCollection<PasswordItemViewModel>(group.OrderByDescending(password => password.Id))));

        foreach (var group in favoriteGroups)
        {
            this.FavoritePasswordGroups.Add(group);
        }

        // Load logo images for favorite password items
        foreach (var group in this.FavoritePasswordGroups)
        {
            foreach (var item in group.Passwords)
            {
                item.LargeLogoImage ??= await LogosService.GetLogoImageAsync(item.LogoImageFileName, LogoImageSize.Large);
            }
        }
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
