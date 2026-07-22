using System;
using System.Collections.Generic;
using Honeypot.Data.Models;
using Microsoft.Data.Sqlite;

namespace Honeypot.Data;

public static class HoneypotDataAccess
{
    private static SqliteConnection? _databaseConnection;

    /// <summary>
    /// Opens the password database and creates required tables if they do not exist.
    /// </summary>
    /// <param name="dbFilePath">The full path of the SQLite database file.</param>
    public static void LoadDatabase(string dbFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dbFilePath);

        if (_databaseConnection is not null)
        {
            return;
        }

        SqliteConnectionStringBuilder builder = new()
        {
            DataSource = dbFilePath,
            Mode = SqliteOpenMode.ReadWriteCreate
        };

        SqliteConnection connection = new(builder.ToString());

        try
        {
            connection.Open();

            _databaseConnection = connection;

            using SqliteCommand createCategoryTable = _databaseConnection.CreateCommand();
            createCategoryTable.CommandText =
                """
                CREATE TABLE IF NOT EXISTS passwordCategories (
                    id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE,
                    title TEXT,
                    icon TEXT,
                    timeorder INTEGER
                );
                """;
            createCategoryTable.ExecuteNonQuery();

            using SqliteCommand createPasswordsTable = _databaseConnection.CreateCommand();
            createPasswordsTable.CommandText =
                """
                CREATE TABLE IF NOT EXISTS passwords (
                    id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE,
                    categoryid INTEGER DEFAULT -1,
                    account TEXT,
                    password TEXT,
                    firstletter TEXT,
                    name TEXT,
                    createdate TEXT,
                    editdate TEXT,
                    website TEXT,
                    note TEXT,
                    favorite INTEGER DEFAULT 0,
                    logo TEXT,
                    thirdpartyid INTEGER DEFAULT -1
                );
                """;
            createPasswordsTable.ExecuteNonQuery();

            // Add this column for old databases created by earlier versions.
            EnsureColumnExists("passwords", "thirdpartyid", "INTEGER DEFAULT -1");
        }
        catch
        {
            connection.Dispose();
            _databaseConnection = null;
            throw;
        }
    }

    /// <summary>
    /// Closes the current database connection.
    /// </summary>
    public static void CloseDatabase()
    {
        _databaseConnection?.Close();
        _databaseConnection?.Dispose();
        _databaseConnection = null;
    }

    /// <summary>
    /// Adds a column to a table if the column does not exist.
    /// </summary>
    /// <param name="tableName">The table name to check.</param>
    /// <param name="columnName">The column name to add.</param>
    /// <param name="columnDefinition">The SQLite column definition, including type and default value.</param>
    private static void EnsureColumnExists(string tableName, string columnName, string columnDefinition)
    {
        if (tableName is not "passwords" and not "passwordCategories")
        {
            throw new ArgumentException("Unknown table name.", nameof(tableName));
        }

        SqliteConnection connection = _databaseConnection ?? throw new InvalidOperationException("Database is not loaded.");

        bool columnExists = false;

        {
            using SqliteCommand pragmaCommand = connection.CreateCommand();
            pragmaCommand.CommandText = $"PRAGMA table_info(\"{tableName}\");";

            using SqliteDataReader reader = pragmaCommand.ExecuteReader();

            while (reader.Read())
            {
                string existingColumnName = reader.GetString(1);
                if (existingColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                {
                    columnExists = true;
                    break;
                }
            }
        }

        if (columnExists)
        {
            return;
        }

        using SqliteCommand alterCommand = connection.CreateCommand();
        alterCommand.CommandText =
            $"""
                ALTER TABLE "{tableName}"
                ADD COLUMN "{columnName}" {columnDefinition};
                """;
        alterCommand.ExecuteNonQuery();
    }

    /// <summary>
    /// Gets passwords. If categoryId is less than 0, all passwords are returned. Otherwise, only passwords in the specified category are returned.
    /// </summary>
    /// <param name="categoryId">The category id used to filter passwords. Use a negative value to get all passwords.</param>
    /// <returns>A list of passwords.</returns>
    public static List<PasswordModel> GetPasswords(long categoryId = -1)
    {
        SqliteConnection connection = _databaseConnection ?? throw new InvalidOperationException("Database is not loaded.");

        List<PasswordModel> results = [];

        using SqliteCommand selectCommand = connection.CreateCommand();

        selectCommand.CommandText = categoryId >= 0
            ? """
                  SELECT id, categoryid, account, password, firstletter, name, createdate, editdate, website, note, favorite, logo, thirdpartyid 
                  FROM passwords 
                  WHERE categoryid = $categoryid;
                  """
            : """
                  SELECT id, categoryid, account, password, firstletter, name, createdate, editdate, website, note, favorite, logo, thirdpartyid 
                  FROM passwords;
                  """;
        if (categoryId >= 0)
        {
            selectCommand.Parameters.AddWithValue("$categoryid", categoryId);
        }

        using SqliteDataReader reader = selectCommand.ExecuteReader();

        while (reader.Read())
        {
            string firstLetter = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);

            results.Add(new PasswordModel
            {
                Id = reader.IsDBNull(0) ? -1 : reader.GetInt64(0),
                CategoryId = reader.IsDBNull(1) ? -1 : reader.GetInt64(1),
                Account = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                Password = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                FirstLetter = string.IsNullOrWhiteSpace(firstLetter) ? '#' : firstLetter[0],
                Name = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                CreateDate = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                EditDate = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                Website = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                Note = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                Favorite = reader.IsDBNull(10) ? 0 : reader.GetInt32(10),
                Logo = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                ThirdPartyId = reader.IsDBNull(12) ? -1 : reader.GetInt64(12)
            });
        }

        return results;
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
    /// <param name="logo">The logo path or logo key.</param>
    /// <returns>The id of the newly added password.</returns>
    public static long AddPassword(long categoryId, string account, string password, long thirdPartyId, string firstLetter, string name, string createDate, string website, string note, bool favorite, string logo)
    {
        SqliteConnection connection = _databaseConnection ?? throw new InvalidOperationException("Database is not loaded.");

        using SqliteCommand insertCommand = connection.CreateCommand();
        insertCommand.CommandText =
            """
                INSERT INTO passwords (categoryid, account, password, firstletter, name, createdate, website, note, favorite, logo, thirdpartyid)
                VALUES ($categoryid, $account, $password, $firstletter, $name, $createdate, $website, $note, $favorite, $logo, $thirdpartyid);
                SELECT last_insert_rowid();
                """;
        insertCommand.Parameters.AddWithValue("$categoryid", categoryId);
        insertCommand.Parameters.AddWithValue("$account", account);
        insertCommand.Parameters.AddWithValue("$password", password);
        insertCommand.Parameters.AddWithValue("$firstletter", firstLetter);
        insertCommand.Parameters.AddWithValue("$name", name);
        insertCommand.Parameters.AddWithValue("$createdate", createDate);
        insertCommand.Parameters.AddWithValue("$website", website);
        insertCommand.Parameters.AddWithValue("$note", note);
        insertCommand.Parameters.AddWithValue("$favorite", favorite ? 1 : 0);
        insertCommand.Parameters.AddWithValue("$logo", logo);
        insertCommand.Parameters.AddWithValue("$thirdpartyid", thirdPartyId);

        var result = insertCommand.ExecuteScalar() ?? throw new InvalidOperationException("Failed to get the id of the newly added password.");

        return Convert.ToInt64(result);
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
    /// <param name="logo">The new logo path or logo key.</param>
    /// <returns><see langword="true"/> if the password was updated; otherwise, <see langword="false"/>.</returns>
    public static bool UpdatePassword(long id, long categoryId, string account, string password, long thirdPartyId, string firstLetter, string name, string editDate, string website, string note, bool favorite, string logo)
    {
        SqliteConnection connection = _databaseConnection ?? throw new InvalidOperationException("Database is not loaded.");

        using SqliteCommand updateCommand = connection.CreateCommand();
        updateCommand.CommandText =
            """
                UPDATE passwords
                SET
                    categoryid = $categoryid,
                    account = $account,
                    password = $password,
                    firstletter = $firstletter,
                    name = $name,
                    editdate = $editdate,
                    website = $website,
                    note = $note,
                    favorite = $favorite,
                    logo = $logo,
                    thirdpartyid = $thirdpartyid
                WHERE id = $id;
                """;
        updateCommand.Parameters.AddWithValue("$categoryid", categoryId);
        updateCommand.Parameters.AddWithValue("$account", account);
        updateCommand.Parameters.AddWithValue("$password", password);
        updateCommand.Parameters.AddWithValue("$firstletter", firstLetter);
        updateCommand.Parameters.AddWithValue("$name", name);
        updateCommand.Parameters.AddWithValue("$editdate", editDate);
        updateCommand.Parameters.AddWithValue("$website", website);
        updateCommand.Parameters.AddWithValue("$note", note);
        updateCommand.Parameters.AddWithValue("$favorite", favorite ? 1 : 0);
        updateCommand.Parameters.AddWithValue("$logo", logo);
        updateCommand.Parameters.AddWithValue("$thirdpartyid", thirdPartyId);
        updateCommand.Parameters.AddWithValue("$id", id);

        return updateCommand.ExecuteNonQuery() > 0;
    }

    /// <summary>
    /// Deletes a password and clears third-party login links that point to it.
    /// </summary>
    /// <param name="id">The password id to delete.</param>
    /// <returns><see langword="true"/> if the password was deleted; otherwise, <see langword="false"/>.</returns>
    public static bool DeletePassword(long id)
    {
        SqliteConnection connection = _databaseConnection ?? throw new InvalidOperationException("Database is not loaded.");

        using SqliteTransaction transaction = connection.BeginTransaction();

        using SqliteCommand updateCommand = connection.CreateCommand();
        updateCommand.Transaction = transaction;
        updateCommand.CommandText =
            """
                UPDATE passwords
                SET thirdpartyid = -1
                WHERE thirdpartyid = $id;
                """;
        updateCommand.Parameters.AddWithValue("$id", id);
        updateCommand.ExecuteNonQuery();

        using SqliteCommand deleteCommand = connection.CreateCommand();
        deleteCommand.Transaction = transaction;
        deleteCommand.CommandText =
            """
                DELETE FROM passwords
                WHERE id = $id;
                """;
        deleteCommand.Parameters.AddWithValue("$id", id);
        int deletedRows = deleteCommand.ExecuteNonQuery();

        transaction.Commit();

        return deletedRows > 0;
    }

    /// <summary>
    /// Marks a password as favorite or not favorite.
    /// </summary>
    /// <param name="id">The password id to update.</param>
    /// <param name="favorite">True to mark it as favorite, or false to remove it from favorites.</param>
    /// <returns><see langword="true"/> if the password was updated; otherwise, <see langword="false"/>.</returns>
    public static bool SetPasswordFavorite(long id, bool favorite)
    {
        SqliteConnection connection = _databaseConnection ?? throw new InvalidOperationException("Database is not loaded.");

        using SqliteCommand updateCommand = connection.CreateCommand();
        updateCommand.CommandText =
            """
                UPDATE passwords
                SET favorite = $favorite
                WHERE id = $id;
                """;
        updateCommand.Parameters.AddWithValue("$favorite", favorite ? 1 : 0);
        updateCommand.Parameters.AddWithValue("$id", id);

        return updateCommand.ExecuteNonQuery() > 0;
    }

    /// <summary>
    /// Gets all password categories ordered by their stored order value.
    /// </summary>
    /// <returns>A list of password categories.</returns>
    public static List<CategoryModel> GetCategories()
    {
        SqliteConnection connection = _databaseConnection ?? throw new InvalidOperationException("Database is not loaded.");

        List<CategoryModel> results = [];

        using SqliteCommand selectCommand = connection.CreateCommand();
        selectCommand.CommandText =
            """
                SELECT id, title, icon, timeorder
                FROM passwordCategories
                ORDER BY timeorder DESC;
                """;

        using SqliteDataReader reader = selectCommand.ExecuteReader();

        while (reader.Read())
        {
            results.Add(new CategoryModel
            {
                Id = reader.IsDBNull(0) ? -1 : reader.GetInt64(0),
                Title = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                Icon = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                Order = reader.IsDBNull(3) ? 0 : reader.GetInt64(3)
            });
        }

        return results;
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
        SqliteConnection connection = _databaseConnection ?? throw new InvalidOperationException("Database is not loaded.");

        using SqliteCommand insertCommand = connection.CreateCommand();
        insertCommand.CommandText =
            """
                INSERT INTO passwordCategories (title, icon, timeorder)
                VALUES ($title, $icon, $order);
                SELECT last_insert_rowid();
                """;
        insertCommand.Parameters.AddWithValue("$title", title);
        insertCommand.Parameters.AddWithValue("$icon", icon);
        insertCommand.Parameters.AddWithValue("$order", sortOrder);

        var result = insertCommand.ExecuteScalar() ?? throw new InvalidOperationException("Failed to get the id of the newly added category.");

        return Convert.ToInt64(result);
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
        SqliteConnection connection = _databaseConnection ?? throw new InvalidOperationException("Database is not loaded.");

        using SqliteCommand updateCommand = connection.CreateCommand();
        updateCommand.CommandText =
            """
                UPDATE passwordCategories
                SET
                    title = $title,
                    icon = $icon,
                    timeorder = $order
                WHERE id = $id;
                """;
        updateCommand.Parameters.AddWithValue("$title", title);
        updateCommand.Parameters.AddWithValue("$icon", icon);
        updateCommand.Parameters.AddWithValue("$order", sortOrder);
        updateCommand.Parameters.AddWithValue("$id", id);

        return updateCommand.ExecuteNonQuery() > 0;
    }

    /// <summary>
    /// Deletes a category and moves its passwords to the uncategorized group.
    /// </summary>
    /// <param name="id">The category id to delete.</param>
    /// <returns><see langword="true"/> if the category was deleted; otherwise, <see langword="false"/>.</returns>
    public static bool DeleteCategory(long id)
    {
        SqliteConnection connection = _databaseConnection ?? throw new InvalidOperationException("Database is not loaded.");

        using SqliteTransaction transaction = connection.BeginTransaction();

        using SqliteCommand updateCommand = connection.CreateCommand();
        updateCommand.Transaction = transaction;
        updateCommand.CommandText =
            """
                UPDATE passwords
                SET categoryid = -1
                WHERE categoryid = $id;
                """;
        updateCommand.Parameters.AddWithValue("$id", id);
        updateCommand.ExecuteNonQuery();

        using SqliteCommand deleteCommand = connection.CreateCommand();
        deleteCommand.Transaction = transaction;
        deleteCommand.CommandText =
            """
                DELETE FROM passwordCategories
                WHERE id = $id;
                """;
        deleteCommand.Parameters.AddWithValue("$id", id);
        int deletedRows = deleteCommand.ExecuteNonQuery();

        transaction.Commit();

        return deletedRows > 0;
    }
}
