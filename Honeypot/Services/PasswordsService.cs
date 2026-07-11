using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Honeypot.Data;
using Honeypot.Data.Models;
using Windows.Storage;

namespace Honeypot.Services;

public static class PasswordsService
{
    /// <summary>
    /// Initializes the password service by creating the required application folders,
    /// creating the database file if it does not exist, and opening the database connection.
    /// </summary>
    /// <returns>A task that represents the asynchronous initialization operation.</returns>
    public static async Task InitializeAsync()
    {
        string documentsFolderPath = UserDataPaths.GetDefault().Documents;
        StorageFolder documentsFolder = await StorageFolder.GetFolderFromPathAsync(documentsFolderPath);
        StorageFolder noMewingFolder = await documentsFolder.CreateFolderAsync("NoMewing", CreationCollisionOption.OpenIfExists);
        StorageFolder honeypotFolder = await noMewingFolder.CreateFolderAsync("Honeypot", CreationCollisionOption.OpenIfExists);
        StorageFile databaseFile = await honeypotFolder.CreateFileAsync("Honeypot.db", CreationCollisionOption.OpenIfExists);

        HoneypotDataAccess.LoadDatabase(databaseFile.Path);
    }

    /// <summary>
    /// Closes the password service and releases the current database connection.
    /// </summary>
    public static void Close()
    {
        HoneypotDataAccess.CloseDatabase();
    }

    /// <summary>
    /// Gets passwords from the database.
    /// </summary>
    /// <param name="categoryId">
    /// The category id used to filter passwords. Use a negative value to get all passwords.
    /// </param>
    /// <returns>A list of password models.</returns>
    public static IReadOnlyList<PasswordModel> GetPasswords(long categoryId = -1)
    {
        return HoneypotDataAccess.GetPasswords(categoryId);
    }

    /// <summary>
    /// Gets all password categories.
    /// </summary>
    /// <returns>A list of category models.</returns>
    public static IReadOnlyList<CategoryModel> GetCategories()
    {
        return HoneypotDataAccess.GetCategories();
    }

    /// <summary>
    /// Adds a password.
    /// </summary>
    /// <returns>The id of the newly added password.</returns>
    public static long AddPassword(long categoryId, string account, string password, long thirdPartyId, string firstLetter, string name, string createDate, string website, string note, bool favorite, string logoFileName)
    {
        return HoneypotDataAccess.AddPassword(categoryId, account, password, thirdPartyId, firstLetter, name, createDate, website, note, favorite, logoFileName);
    }

    /// <summary>
    /// Updates a password.
    /// </summary>
    public static bool UpdatePassword(long id, long categoryId, string account, string password, long thirdPartyId, string firstLetter, string name, string editDate, string website, string note, bool favorite, string logoFileName)
    {
        return HoneypotDataAccess.UpdatePassword(id, categoryId, account, password, thirdPartyId, firstLetter, name, editDate, website, note, favorite, logoFileName);
    }

    /// <summary>
    /// Deletes a password and its logo image if needed.
    /// </summary>
    public static async Task<bool> DeletePasswordAsync(long id, string logoFileName)
    {
        bool deleted = HoneypotDataAccess.DeletePassword(id);

        if (!deleted)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(logoFileName))
        {
            await LogosService.DeleteLogoImageAsync(logoFileName);
        }

        return true;
    }

    /// <summary>
    /// Marks a password as favorite or not favorite.
    /// </summary>
    public static bool SetPasswordFavorite(long id, bool favorite)
    {
        return HoneypotDataAccess.SetPasswordFavorite(id, favorite);
    }

    /// <summary>
    /// Adds a category.
    /// </summary>
    public static long AddCategory(string title, string icon, long sortOrder)
    {
        return HoneypotDataAccess.AddCategory(title, icon, sortOrder);
    }

    /// <summary>
    /// Updates a category.
    /// </summary>
    public static bool UpdateCategory(long id, string title, string icon, long sortOrder)
    {
        return HoneypotDataAccess.UpdateCategory(id, title, icon, sortOrder);
    }

    /// <summary>
    /// Deletes a category and moves its passwords to the uncategorized group.
    /// </summary>
    public static bool DeleteCategory(long id)
    {
        return HoneypotDataAccess.DeleteCategory(id);
    }
}
