using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Honeypot.Data.Models;
using Microsoft.Data.Sqlite;

namespace Honeypot.Data
{
    public static class PasswordsDataAccess
    {
        private static SqliteConnection? _passwordsDb;

        /// <summary>
        /// Opens the password database and creates required tables if they do not exist.
        /// </summary>
        /// <param name="dbFilePath">The full path of the SQLite database file.</param>
        public static void LoadDatabase(string dbFilePath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(dbFilePath);

            if (_passwordsDb is not null)
            {
                return;
            }

            SqliteConnectionStringBuilder builder = new()
            {
                DataSource = dbFilePath,
                Mode = SqliteOpenMode.ReadWriteCreate
            };

            _passwordsDb = new SqliteConnection(builder.ToString());
            _passwordsDb.Open();

            using SqliteCommand createCategoryTable = _passwordsDb.CreateCommand();
            createCategoryTable.CommandText =
                """
                CREATE TABLE IF NOT EXISTS passwordCategories (
                    id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE,
                    title TEXT,
                    icon TEXT,
                    timeorder INTEGER);
                """;
            createCategoryTable.ExecuteNonQuery();

            using SqliteCommand createPasswordsTable = _passwordsDb.CreateCommand();
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
                    thirdpartyid INTEGER DEFAULT -1);
                """;
            createPasswordsTable.ExecuteNonQuery();

            // Add this column for old databases created by earlier versions.
            EnsureColumnExists("passwords", "thirdpartyid", "INTEGER DEFAULT -1");
        }

        /// <summary>
        /// Closes the current database connection.
        /// </summary>
        public static void CloseDatabase()
        {
            _passwordsDb?.Close();
            _passwordsDb?.Dispose();
            _passwordsDb = null;
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

            SqliteConnection connection = _passwordsDb ?? throw new InvalidOperationException("Database is not loaded.");

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
        /// Gets passwords. If categoryId is greater than 0, only passwords in that category are returned.
        /// </summary>
        /// <param name="categoryId">The category id used to filter passwords. Use -1 to get all passwords.</param>
        /// <returns>A list of passwords.</returns>
        public static List<PasswordDataModel> GetPasswords(int categoryId = -1)
        {
            SqliteConnection connection = _passwordsDb ?? throw new InvalidOperationException("Database is not loaded.");

            List<PasswordDataModel> results = [];

            using SqliteCommand selectCommand = _passwordsDb.CreateCommand();

            selectCommand.CommandText = categoryId > 0 ? @"SELECT * FROM passwords WHERE categoryid=$categoryid" : @"SELECT * FROM passwords";
            if (categoryId > 0)
            {
                selectCommand.Parameters.AddWithValue("$categoryid", categoryId);
            }

            using SqliteDataReader query = selectCommand.ExecuteReader();
            while (query?.Read() == true)
            {
                PasswordDataModel item = new PasswordDataModel();
                item.Id = query.IsDBNull(0) ? -1 : query.GetInt32(0);
                item.CategoryId = query.IsDBNull(1) ? -1 : query.GetInt32(1);
                item.Account = query.IsDBNull(2) ? string.Empty : query.GetString(2);
                item.Password = query.IsDBNull(3) ? string.Empty : query.GetString(3);
                item.Name = query.IsDBNull(5) ? string.Empty : query.GetString(5);
                item.CreateDate = query.IsDBNull(6) ? string.Empty : query.GetString(6);
                item.EditDate = query.IsDBNull(7) ? string.Empty : query.GetString(7);
                item.Website = query.IsDBNull(8) ? string.Empty : query.GetString(8);
                item.Note = query.IsDBNull(9) ? string.Empty : query.GetString(9);
                item.Favorite = query.IsDBNull(10) ? 0 : query.GetInt32(10);
                item.Logo = query.IsDBNull(11) ? null : query.GetString(11);
                var firstLetter = query.IsDBNull(4) ? string.Empty : query.GetString(4);
                item.FirstLetter = (string.IsNullOrWhiteSpace(firstLetter) || firstLetter.Length <= 0) ? '#' : firstLetter[0];
                item.ThirdPartyId = query.IsDBNull(12) ? -1 : query.GetInt32(12);
                results.Add(item);
            }

            return results;
        }

        /// <summary>
        /// 添加一个密码
        /// </summary>
        /// <param name="categoryid"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="thirdPartyId"></param>
        /// <param name="firstLetter"></param>
        /// <param name="name"></param>
        /// <param name="createDate"></param>
        /// <param name="website"></param>
        /// <param name="note"></param>
        /// <param name="favorite"></param>
        /// <param name="logo"></param>
        public static void AddPassword(int categoryid, string account, string password, int thirdPartyId, string firstLetter, string name, string createDate, string website, string note, bool favorite, string logo)
        {
            using SqliteCommand insertCommand = _passwordsDb.CreateCommand();
            insertCommand.CommandText =
            @"
                    INSERT INTO passwords(categoryid,account,password,firstletter,name,createdate,website,note,favorite,logo,thirdpartyid) 
                    VALUES($categoryid,$account,$password,$firstletter,$name,$createdate,$website,$note,$favorite,$logo,$thirdpartyid);
                ";

            insertCommand.Parameters.AddWithValue("$categoryid", categoryid);
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
            insertCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新指定ID的密码的信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="categoryid"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="thirdPartyId"></param>
        /// <param name="firstLetter"></param>
        /// <param name="name"></param>
        /// <param name="editDate"></param>
        /// <param name="website"></param>
        /// <param name="note"></param>
        /// <param name="favorite"></param>
        /// <param name="logo"></param>
        public static void UpdatePassword(long id, int categoryid, string account, string password, int thirdPartyId, string firstLetter, string name, string editDate, string website, string note, bool favorite, string logo)
        {
            using SqliteCommand updateCommand = _passwordsDb.CreateCommand();
            updateCommand.CommandText = @"UPDATE passwords SET categoryid=$categoryid,account=$account,password=$password,firstletter=$firstletter,name=$name,editdate=$editdate,website=$website,note=$note,favorite=$favorite,logo=$logo,thirdpartyid=$thirdpartyid WHERE id=$id;";

            updateCommand.Parameters.AddWithValue("$categoryid", categoryid);
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
            updateCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 收藏/取消收藏密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="favorite"></param>
        public static void FavoritePassword(long id, bool favorite)
        {
            using SqliteCommand updateCommand = _passwordsDb.CreateCommand();
            updateCommand.CommandText = @"UPDATE passwords SET favorite=$favorite WHERE id=$id;";

            updateCommand.Parameters.AddWithValue("$favorite", favorite ? 1 : 0);
            updateCommand.Parameters.AddWithValue("$id", id);
            updateCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 删除指定ID的密码，并将passwords表中所有设置了该密码ID的第三方登录ID改为-1
        /// </summary>
        /// <param name="id"></param>
        public static void DeletePassword(long id)
        {
            using SqliteCommand deleteCommand = _passwordsDb.CreateCommand();
            deleteCommand.CommandText = @"DELETE FROM passwords WHERE id=$id;";

            deleteCommand.Parameters.AddWithValue("$id", id);
            deleteCommand.ExecuteNonQuery();

            SqliteCommand updateCommand = _passwordsDb.CreateCommand();
            updateCommand.CommandText = @"UPDATE passwords SET thirdpartyid=-1 WHERE thirdpartyid=$id;";

            updateCommand.Parameters.AddWithValue("$id", id);
            updateCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取所有的密码分类列表
        /// </summary>
        /// <returns></returns>
        public static List<CategoryDataModel> GetCategories()
        {
            List<CategoryDataModel> results = new List<CategoryDataModel>();

            using SqliteCommand selectCommand = _passwordsDb.CreateCommand();
            selectCommand.CommandText = @"SELECT * FROM passwordCategories ORDER BY timeorder ASC;";

            using SqliteDataReader query = selectCommand.ExecuteReader();
            while (query?.Read() == true)
            {
                CategoryDataModel item = new CategoryDataModel();
                item.Id = query.IsDBNull(0) ? -1 : query.GetInt32(0);
                item.Title = query.IsDBNull(1) ? string.Empty : query.GetString(1);
                item.Icon = query.IsDBNull(2) ? string.Empty : query.GetString(2);
                item.Order = query.IsDBNull(3) ? 0 : query.GetInt64(3);
                results.Add(item);
            }

            return results;
        }

        /// <summary>
        /// 添加一个分类
        /// </summary>
        /// <param name="title"></param>
        /// <param name="icon"></param>
        public static void AddCategory(string title, string icon, long ticksAsOrder)
        {
            using SqliteCommand insertCommand = _passwordsDb.CreateCommand();
            insertCommand.CommandText = @"INSERT INTO passwordCategories(title, icon, timeorder) VALUES($title, $icon, $order);";

            insertCommand.Parameters.AddWithValue("$title", title);
            insertCommand.Parameters.AddWithValue("$icon", icon);
            insertCommand.Parameters.AddWithValue("$order", ticksAsOrder);
            insertCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 修改分类的属性
        /// </summary>
        /// <param name="title"></param>
        /// <param name="icon"></param>
        public static void UpdateCategory(int id, string title, string icon, long ticksAsOrder)
        {
            using SqliteCommand updateCommand = _passwordsDb.CreateCommand();
            updateCommand.CommandText = @"UPDATE passwordCategories SET title=$title, icon=$icon, timeorder=$order WHERE id=$id;";

            updateCommand.Parameters.AddWithValue("$title", title);
            updateCommand.Parameters.AddWithValue("$icon", icon);
            updateCommand.Parameters.AddWithValue("$order", ticksAsOrder);
            updateCommand.Parameters.AddWithValue("$id", id);
            updateCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 删除一个分类，并将passwords表中所有属于该分类的密码分类改为空
        /// </summary>
        /// <param name="id"></param>
        public static void DeleteCategory(int id)
        {
            using SqliteCommand deleteCommand = _passwordsDb.CreateCommand();
            deleteCommand.CommandText = @"DELETE FROM passwordCategories WHERE id=$id;";

            deleteCommand.Parameters.AddWithValue("$id", id);
            deleteCommand.ExecuteNonQuery();

            SqliteCommand updateCommand = _passwordsDb.CreateCommand();
            updateCommand.CommandText = @"UPDATE passwords SET categoryid=-1 WHERE categoryid=$id;";

            updateCommand.Parameters.AddWithValue("$id", id);
            updateCommand.ExecuteNonQuery();
        }
    }
}
