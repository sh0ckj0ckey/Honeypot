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
    /// <param name="categoryId">The category id of the password. Use -1 if it has no category.</param>
    /// <param name="account">The account name or login name.</param>
    /// <param name="password">The password text to store.</param>
    /// <param name="thirdPartyId">The related third-party login password id. Use -1 if it has none.</param>
    /// <param name="firstLetter">The first letter used for grouping or sorting.</param>
    /// <param name="name">The display name of the password item.</param>
    /// <param name="createDate">The creation date string.</param>
    /// <param name="website">The related website.</param>
    /// <param name="note">The note text.</param>
    /// <param name="favorite">Whether this password is marked as favorite.</param>
    /// <param name="logoFileName">The stored logo file name.</param>
    /// <returns>The id of the newly added password.</returns>
    public static long AddPassword(long categoryId, string account, string password, long thirdPartyId, string firstLetter, string name, string createDate, string website, string note, bool favorite, string logoFileName)
    {
        return HoneypotDataAccess.AddPassword(categoryId, account, password, thirdPartyId, firstLetter, name, createDate, website, note, favorite, logoFileName);
    }

    /// <summary>
    /// Updates a password by id.
    /// </summary>
    /// <param name="id">The password id to update.</param>
    /// <param name="categoryId">The new category id. Use -1 if it has no category.</param>
    /// <param name="account">The new account name or login name.</param>
    /// <param name="password">The new password text to store.</param>
    /// <param name="thirdPartyId">The new related third-party login password id. Use -1 if it has none.</param>
    /// <param name="firstLetter">The new first letter used for grouping or sorting.</param>
    /// <param name="name">The new display name.</param>
    /// <param name="editDate">The edit date string.</param>
    /// <param name="website">The new related website.</param>
    /// <param name="note">The new note text.</param>
    /// <param name="favorite">Whether this password is marked as favorite.</param>
    /// <param name="logoFileName">The stored logo file name.</param>
    /// <returns><see langword="true"/> if the password was updated; otherwise, <see langword="false"/>.</returns>
    public static bool UpdatePassword(long id, long categoryId, string account, string password, long thirdPartyId, string firstLetter, string name, string editDate, string website, string note, bool favorite, string logoFileName)
    {
        return HoneypotDataAccess.UpdatePassword(id, categoryId, account, password, thirdPartyId, firstLetter, name, editDate, website, note, favorite, logoFileName);
    }

    /// <summary>
    /// Deletes a password by id and starts a best-effort background cleanup for its logo image if needed.
    /// </summary>
    /// <param name="id">The password id to delete.</param>
    /// <param name="logoFileName">The stored logo file name.</param>
    /// <returns>
    /// <see langword="true"/> if the password was deleted; otherwise, <see langword="false"/>.
    /// The return value does not indicate whether the logo image cleanup succeeded.
    /// </returns>
    public static bool DeletePassword(long id, string logoFileName)
    {
        bool deleted = HoneypotDataAccess.DeletePassword(id);

        if (!deleted)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(logoFileName))
        {
            _ = DeleteLogoImageSilentlyAsync(logoFileName);
        }

        return true;

        static async Task DeleteLogoImageSilentlyAsync(string fileName)
        {
            try
            {
                await LogosService.DeleteLogoImageAsync(fileName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
        }
    }

    /// <summary>
    /// Marks a password as favorite or not favorite.
    /// </summary>
    /// <param name="id">The password id to update.</param>
    /// <param name="favorite">True to mark it as favorite, or false to remove it from favorites.</param>
    /// <returns><see langword="true"/> if the password was updated; otherwise, <see langword="false"/>.</returns>
    public static bool SetPasswordFavorite(long id, bool favorite)
    {
        return HoneypotDataAccess.SetPasswordFavorite(id, favorite);
    }

    /// <summary>
    /// Adds a password category.
    /// </summary>
    /// <param name="title">The category title.</param>
    /// <param name="icon">The category icon path or icon key.</param>
    /// <param name="sortOrder">The custom order value, usually based on ticks.</param>
    /// <returns>The id of the newly added category.</returns>
    public static long AddCategory(string title, string icon, long sortOrder)
    {
        return HoneypotDataAccess.AddCategory(title, icon, sortOrder);
    }

    /// <summary>
    /// Updates a password category by id.
    /// </summary>
    /// <param name="id">The category id to update.</param>
    /// <param name="title">The new category title.</param>
    /// <param name="icon">The new category icon path or icon key.</param>
    /// <param name="sortOrder">The new custom order value.</param>
    /// <returns><see langword="true"/> if the category was updated; otherwise, <see langword="false"/>.</returns>
    public static bool UpdateCategory(long id, string title, string icon, long sortOrder)
    {
        return HoneypotDataAccess.UpdateCategory(id, title, icon, sortOrder);
    }

    /// <summary>
    /// Deletes a category and moves its passwords to the uncategorized group.
    /// </summary>
    /// <param name="id">The category id to delete.</param>
    /// <returns><see langword="true"/> if the category was deleted; otherwise, <see langword="false"/>.</returns>
    public static bool DeleteCategory(long id)
    {
        return HoneypotDataAccess.DeleteCategory(id);
    }
}
