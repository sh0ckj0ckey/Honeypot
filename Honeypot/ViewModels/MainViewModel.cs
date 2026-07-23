using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Honeypot.Data.Models;
using Honeypot.Helpers;
using Honeypot.Services;

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
    /// Gets the view model that provides items for the main navigation view.
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
    /// Gets the password items in the currently selected category, ordered from newest to oldest.
    /// </summary>
    public ObservableCollection<PasswordItemViewModel> CurrentPasswordItems { get; } = [];

    /// <summary>
    /// Gets the password items in the currently selected category, grouped by first letter.
    /// </summary>
    public ObservableCollection<PasswordGroupViewModel> CurrentPasswordGroups { get; } = [];

    /// <summary>
    /// Gets favorite password items grouped by category.
    /// </summary>
    public ObservableCollection<PasswordGroupViewModel> FavoritePasswordGroups { get; } = [];

    /// <summary>
    /// Gets a value indicating whether the main view model is currently loading data.
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }

    /// <summary>
    /// Gets the category currently used to filter the password collections.
    /// A <see langword="null"/> value means that all passwords are included.
    /// </summary>
    public CategoryItemViewModel? SelectedCategory
    {
        get => _selectedCategory;
        private set => SetProperty(ref _selectedCategory, value);
    }

    /// <summary>
    /// Gets the password item currently selected in the password list.
    /// </summary>
    public PasswordItemViewModel? SelectedPassword
    {
        get => _selectedPassword;
        internal set => SetProperty(ref _selectedPassword, value);
    }

    /// <summary>
    /// Loads categories and passwords from the password service and rebuilds the navigation, current-password, and favorite-password collections.
    /// This method has no effect when the data has already been loaded or a load operation is currently in progress.
    /// </summary>
    /// <remarks>
    /// The password service must be initialized before this method is called.
    /// Logo image loading is started in the background and may continue after this method has completed.
    /// </remarks>
    /// <returns>A task that represents the data-loading operation.</returns>
    public async Task LoadAsync()
    {
        if (_isLoaded || this.IsLoading)
        {
            return;
        }

        try
        {
            this.Clear();

            this.IsLoading = true;

            List<CategoryItemViewModel> categories = [];
            List<PasswordItemViewModel> passwords = [];
            Dictionary<long, long> passwordToCategory = [];
            Dictionary<long, long> passwordToThirdParty = [];

            await Task.Run(() =>
            {
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

                categories.Sort((x, y) => y.Order.CompareTo(x.Order));
                passwords.Sort((x, y) => y.Id.CompareTo(x.Id));
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

            _isLoaded = true;

            this.UpdateCurrentPasswords();
            this.UpdateFavoritePasswords();

            // Update the navigation view with the loaded categories
            this.Navigation.UpdateCategories(this.AllCategories);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine(ex);
        }
        finally
        {
            this.IsLoading = false;
        }
    }

    /// <summary>
    /// Clears all loaded categories, passwords, derived collections, and current selections,
    /// and resets the view model to its unloaded state.
    /// </summary>
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

    /// <summary>
    /// Rebuilds the current password item and group collections for the specified category
    /// and starts loading normal logo images in the background.
    /// </summary>
    /// <param name="filterCategoryId">
    /// The category ID used to filter passwords. A negative value includes all passwords.
    /// </param>
    private void UpdateCurrentPasswords(long filterCategoryId = -1)
    {
        this.CurrentPasswordItems.Clear();
        this.CurrentPasswordGroups.Clear();

        if (!_isLoaded)
        {
            return;
        }

        IReadOnlyList<PasswordItemViewModel> filteredPasswords = filterCategoryId < 0
            ? [.. this.AllPasswords]
            : [.. this.AllPasswords.Where(password => password.Category?.Id == filterCategoryId)];

        foreach (var item in filteredPasswords.OrderByDescending(password => password.Id))
        {
            this.CurrentPasswordItems.Add(item);
        }

        var passwordGroups =
            filteredPasswords
            .GroupBy(password => password.FirstLetter)
            .OrderBy(group => group.Key)
            .Select(group => new PasswordGroupViewModel(
                group.Key.ToString(),
                new ObservableCollection<PasswordItemViewModel>(group.OrderByDescending(password => password.Id))));

        foreach (var group in passwordGroups)
        {
            this.CurrentPasswordGroups.Add(group);
        }

        // Load logo images for current password items
        _ = LoadLogoImagesAsync(filteredPasswords);

        static async Task LoadLogoImagesAsync(IReadOnlyList<PasswordItemViewModel> passwords)
        {
            foreach (var password in passwords)
            {
                try
                {
                    password.LogoImage ??= await LogosService.GetLogoImageAsync(password.LogoImageFileName, LogoImageSize.Medium);
                }
                catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            }
        }
    }

    /// <summary>
    /// Rebuilds the favorite password groups by category
    /// and starts loading large logo images in the background.
    /// </summary>
    private void UpdateFavoritePasswords()
    {
        this.FavoritePasswordGroups.Clear();

        if (!_isLoaded)
        {
            return;
        }

        IReadOnlyList<PasswordItemViewModel> favoritePasswords = [.. this.AllPasswords.Where(password => password.Favorite)];

        var favoriteGroups =
            favoritePasswords
            .GroupBy(password => password.Category)
            .OrderByDescending(group => group.Key?.Order ?? long.MaxValue)
            .ThenByDescending(group => group.Key?.Id ?? long.MaxValue)
            .Select(group => new PasswordGroupViewModel(
                group.Key?.Title ?? "UncategorizedCategoryTitle".GetLocalized(),
                group.Key?.Icon ?? "\uEA3A",
                new ObservableCollection<PasswordItemViewModel>(group.OrderByDescending(password => password.Id))));

        foreach (var group in favoriteGroups)
        {
            this.FavoritePasswordGroups.Add(group);
        }

        // Load logo images for favorite password items
        _ = LoadLogoImagesAsync(favoritePasswords);

        static async Task LoadLogoImagesAsync(IReadOnlyList<PasswordItemViewModel> passwords)
        {
            foreach (var item in passwords)
            {
                try
                {
                    item.LargeLogoImage ??= await LogosService.GetLogoImageAsync(item.LogoImageFileName, LogoImageSize.Large);
                }
                catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            }
        }
    }

    /// <summary>
    /// Selects a category, rebuilds the current password collections,
    /// and restores the previously selected password when it remains in the resulting collection.
    /// </summary>
    /// <param name="categoryId">
    /// The category ID to select. A negative value selects all passwords.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the all-passwords view or an existing category was selected;
    /// otherwise, <see langword="false"/> if the view model is not loaded or the specified category does not exist.
    /// </returns>
    public bool SelectCategory(long categoryId)
    {
        if (!_isLoaded)
        {
            return false;
        }

        long selectedPasswordId = this.SelectedPassword?.Id ?? -1;

        if (categoryId < 0)
        {
            this.SelectedCategory = null;
            this.SelectedPassword = null;
            this.UpdateCurrentPasswords();
        }
        else if (_categoriesById.TryGetValue(categoryId, out var category))
        {
            this.SelectedCategory = category;
            this.SelectedPassword = null;
            this.UpdateCurrentPasswords(categoryId);
        }
        else
        {
            return false;
        }

        if (selectedPasswordId > 0)
        {
            this.SelectedPassword = this.CurrentPasswordItems.FirstOrDefault(password => password.Id == selectedPasswordId);
        }

        return true;
    }

    /// <summary>
    /// Searches the current password collection for items whose name or account starts with the specified keyword.
    /// </summary>
    /// <param name="keyword">
    /// The keyword used to search password names and accounts.
    /// </param>
    /// <returns>
    /// A snapshot containing the matching password items. 
    /// The collection is empty when <paramref name="keyword"/> is <see langword="null"/>, empty, or consists only of white-space characters.
    /// </returns>
    public IReadOnlyList<PasswordItemViewModel> SearchCurrentPasswords(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return [];
        }

        return [.. this.CurrentPasswordItems.Where(p =>
            p.Name.StartsWith(keyword, StringComparison.CurrentCultureIgnoreCase) ||
            p.Account.StartsWith(keyword, StringComparison.CurrentCultureIgnoreCase))];
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

    /// <summary>
    /// 添加密码
    /// </summary>
    /// <param name="categoryId"></param>
    /// <param name="account"></param>
    /// <param name="password"></param>
    /// <param name="name"></param>
    /// <param name="website"></param>
    /// <param name="note"></param>
    /// <param name="favorite"></param>
    /// <param name="logoFilePath"></param>
    /// <param name="date"></param>
    public void AddPassword(int categoryId, string account, string password, int thirdPartyId, string name, string website, string note, bool favorite, string logoFilePath, string date = "")
    {
        string firstLetter = PinyinHelper.GetFirstSpell(name.Trim()).ToString();
        if (string.IsNullOrWhiteSpace(date))
        {
            date = DateTime.Now.ToString("yyyy/MM/dd");
        }

        PasswordsDataAccess.AddPassword(categoryId, account, password, thirdPartyId, firstLetter, name, date, website, note, favorite, logoFilePath);
        LoadPasswordsTable();
    }

    /// <summary>
    /// 编辑密码
    /// </summary>
    /// <param name="passwordItem"></param>
    /// <param name="categoryId"></param>
    /// <param name="account"></param>
    /// <param name="password"></param>
    /// <param name="name"></param>
    /// <param name="website"></param>
    /// <param name="note"></param>
    /// <param name="favorite"></param>
    /// <param name="logoFilePath"></param>
    public async void EditPassword(PasswordModel passwordItem, int categoryId, string account, string password, int thirdPartyId, string name, string website, string note, bool favorite, string logoFilePath)
    {
        try
        {
            if (logoFilePath != passwordItem.LogoFileName)
            {
                LogoImageHelper.DeleteLogoImage(passwordItem.LogoFileName);
            }

            string firstLetter = PinyinHelper.GetFirstSpell(name).ToString();
            string date = DateTime.Now.ToString("yyyy/MM/dd");

            PasswordsDataAccess.UpdatePassword(passwordItem.Id, categoryId, account, password, thirdPartyId, firstLetter, name, date, website, note, favorite, logoFilePath);

            passwordItem.Account = account;
            passwordItem.Password = password;
            passwordItem.ThirdPartyId = thirdPartyId;
            passwordItem.FirstLetter = firstLetter[0];
            passwordItem.Name = name;
            passwordItem.EditDate = date;
            passwordItem.Website = website;
            passwordItem.Note = note;
            passwordItem.Favorite = favorite;
            passwordItem.CategoryId = categoryId;
            passwordItem.LogoFileName = logoFilePath;

            passwordItem.NormalLogoImage = await LogoImageHelper.GetLogoImage(logoFilePath, LogoSizeEnum.Medium);
            passwordItem.LargeLogoImage = await LogoImageHelper.GetLogoImage(logoFilePath, LogoSizeEnum.Large);

            UpdatePasswordsList(PasswordsCategoryId, passwordItem.Id);
            UpdateFavorites();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);

            var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
            ShowTipsContentDialog(resourceLoader.GetString("DialogTitleOops"), $"{resourceLoader.GetString("DialogContentWrongEditPasswords")}: {ex.Message}");
        }
    }

    /// <summary>
    /// 删除密码
    /// </summary>
    /// <param name="passwordItem"></param>
    public void DeletePassword(PasswordModel passwordItem)
    {
        try
        {
            LogoImageHelper.DeleteLogoImage(passwordItem.LogoFileName);
            PasswordsDataAccess.DeletePassword(passwordItem.Id);

            LoadPasswordsTable();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);

            var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
            ShowTipsContentDialog(resourceLoader.GetString("DialogTitleOops"), $"{resourceLoader.GetString("DialogContentWrongDeletePasswords")}: {ex.Message}");
        }
    }

    /// <summary>
    /// 添加分类
    /// </summary>
    /// <param name="title"></param>
    /// <param name="icon"></param>
    public void CreateCategory(string title, string icon)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                title = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader().GetString("UnknownCategoryTitle");
            }

            if (string.IsNullOrWhiteSpace(icon))
            {
                icon = "\uE72E";
            }

            PasswordsDataAccess.AddCategory(title, icon, DateTime.Now.Ticks);

            LoadCategoriesTable();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);

            var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
            ShowTipsContentDialog(resourceLoader.GetString("DialogTitleOops"), $"{resourceLoader.GetString("DialogContentWrongAddCategories")}: {ex.Message}");
        }
    }

    /// <summary>
    /// 编辑分类信息
    /// </summary>
    /// <param name="category"></param>
    /// <param name="title"></param>
    /// <param name="icon"></param>
    public void EditCategory(CategoryModel category, string title, string icon)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                title = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader().GetString("UnknownCategoryTitle");
            }

            if (string.IsNullOrWhiteSpace(icon))
            {
                icon = "\uE72E";
            }

            PasswordsDataAccess.UpdateCategory(category.Id, title, icon, category.Order);

            LoadPasswordsTable();
            LoadCategoriesTable();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);

            var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
            ShowTipsContentDialog(resourceLoader.GetString("DialogTitleOops"), $"{resourceLoader.GetString("DialogContentWrongEditCategories")}: {ex.Message}");
        }
    }

    /// <summary>
    /// 将分类移到最前
    /// </summary>
    /// <param name="category"></param>
    public void MoveCategory(CategoryModel category)
    {
        try
        {
            PasswordsDataAccess.UpdateCategory(category.Id, category.Title, category.Icon, DateTime.Now.Ticks);

            LoadCategoriesTable();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);

            var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
            ShowTipsContentDialog(resourceLoader.GetString("DialogTitleOops"), $"{resourceLoader.GetString("DialogContentWrongMoveCategories")}: {ex.Message}");
        }
    }

    /// <summary>
    /// 删除分类
    /// </summary>
    /// <param name="id"></param>
    public void DeleteCategory(int id)
    {
        try
        {
            PasswordsDataAccess.DeleteCategory(id);

            LoadPasswordsTable();
            LoadCategoriesTable();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);

            var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
            ShowTipsContentDialog(resourceLoader.GetString("DialogTitleOops"), $"{resourceLoader.GetString("DialogContentWrongDeleteCategories")}: {ex.Message}");
        }
    }
}
