using System.Collections.Generic;
using System.Threading.Tasks;
using Honeypot.Data;
using Honeypot.Data.Models;

namespace Honeypot.Services;

public class PasswordsService
{
    /// <summary>
    /// Gets passwords from the database.
    /// </summary>
    /// <param name="categoryId">
    /// The category id used to filter passwords. Use a negative value to get all passwords.
    /// </param>
    /// <returns>A list of password models.</returns>
    public IReadOnlyList<PasswordModel> GetPasswords(int categoryId = -1)
    {
        return HoneypotDataAccess.GetPasswords(categoryId);
    }

    /// <summary>
    /// Gets all password categories.
    /// </summary>
    /// <returns>A list of category models.</returns>
    public IReadOnlyList<CategoryModel> GetCategories()
    {
        return HoneypotDataAccess.GetCategories();
    }

    /// <summary>
    /// Adds a password.
    /// </summary>
    /// <returns>The id of the newly added password.</returns>
    public long AddPassword(int categoryId, string account, string password, int thirdPartyId, string firstLetter, string name, string createDate, string website, string note, bool favorite, string logoFileName)
    {
        return HoneypotDataAccess.AddPassword(categoryId, account, password, thirdPartyId, firstLetter, name, createDate, website, note, favorite, logoFileName);
    }

    /// <summary>
    /// Updates a password.
    /// </summary>
    public bool UpdatePassword(long id, int categoryId, string account, string password, int thirdPartyId, string firstLetter, string name, string editDate, string website, string note, bool favorite, string logoFileName)
    {
        return HoneypotDataAccess.UpdatePassword(id, categoryId, account, password, thirdPartyId, firstLetter, name, editDate, website, note, favorite, logoFileName);
    }

    /// <summary>
    /// Deletes a password and its logo image if needed.
    /// </summary>
    public async Task<bool> DeletePasswordAsync(long id, string logoFileName)
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
    public bool SetPasswordFavorite(long id, bool favorite)
    {
        return HoneypotDataAccess.SetPasswordFavorite(id, favorite);
    }

    /// <summary>
    /// Adds a category.
    /// </summary>
    public long AddCategory(string title, string icon, long sortOrder)
    {
        return HoneypotDataAccess.AddCategory(title, icon, sortOrder);
    }

    /// <summary>
    /// Updates a category.
    /// </summary>
    public bool UpdateCategory(int id, string title, string icon, long sortOrder)
    {
        return HoneypotDataAccess.UpdateCategory(id, title, icon, sortOrder);
    }

    /// <summary>
    /// Deletes a category and moves its passwords to the uncategorized group.
    /// </summary>
    public bool DeleteCategory(int id)
    {
        return HoneypotDataAccess.DeleteCategory(id);
    }
}
