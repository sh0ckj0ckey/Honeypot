using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Honeypot.Data.Models;
using Microsoft.Data.Sqlite;
using Windows.Storage;

namespace Honeypot.Data
{
    public static class PasswordsDataAccess
    {
        private static SqliteConnection _passwordsDb = null;

        public static async Task LoadDatabase(StorageFolder folder)
        {
            var file = await folder.CreateFileAsync("Honeypot.db", CreationCollisionOption.OpenIfExists);

            string dbpath = file.Path;
            _passwordsDb = new SqliteConnection($"Filename={dbpath}");
            _passwordsDb.Open();

            string categoryTableCommand =
                "CREATE TABLE IF NOT EXISTS passwordCategories (" +
                    "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE," +
                    "title TEXT," +
                    "icon TEXT," +
                    "timeorder INTEGER);";
            using SqliteCommand createCategoryTable = new SqliteCommand(categoryTableCommand, _passwordsDb);
            createCategoryTable.ExecuteNonQuery();

            string passwordsTableCommand =
                "CREATE TABLE IF NOT EXISTS passwords (" +
                    "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE," +
                    "categoryid INTEGER DEFAULT -1," +
                    "account TEXT," +
                    "password TEXT," +
                    "firstletter TEXT," +
                    "name TEXT," +
                    "createdate TEXT," +
                    "editdate TEXT," +
                    "website TEXT," +
                    "note TEXT," +
                    "favorite INTEGER DEFAULT 0," +
                    "logo TEXT," +
                    "thirdpartyid INTEGER DEFAULT -1);";
            using SqliteCommand createPasswordsTable = new SqliteCommand(passwordsTableCommand, _passwordsDb);
            createPasswordsTable.ExecuteNonQuery();

            // 给现有的表添加字段
            EnsureColumnExists("passwords", "thirdpartyid", "INTEGER DEFAULT -1");
        }

        public static bool IsDatabaseConnected()
        {
            if (_passwordsDb?.State == System.Data.ConnectionState.Open ||
                _passwordsDb?.State == System.Data.ConnectionState.Executing ||
                _passwordsDb?.State == System.Data.ConnectionState.Connecting ||
                _passwordsDb?.State == System.Data.ConnectionState.Fetching)
            {
                return true;
            }

            return false;
        }

        public static void CloseDatabase()
        {
            _passwordsDb?.Close();
            _passwordsDb?.Dispose();
            _passwordsDb = null;
        }

        /// <summary>
        /// 确保指定表中存在某个字段，如果不存在则添加
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columnName">字段名</param>
        /// <param name="columnDefinition">字段定义（类型和默认值）</param>
        private static void EnsureColumnExists(string tableName, string columnName, string columnDefinition)
        {
            // 查询表结构
            string pragmaCommand = $"PRAGMA table_info({tableName});";
            using SqliteCommand pragmaCmd = new SqliteCommand(pragmaCommand, _passwordsDb);
            using SqliteDataReader reader = pragmaCmd.ExecuteReader();

            // 检查字段是否存在
            bool columnExists = false;
            while (reader.Read())
            {
                string existingColumnName = reader.GetString(1); // 第2列是字段名
                if (existingColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                {
                    columnExists = true;
                    break;
                }
            }

            // 如果字段不存在，则添加
            if (!columnExists)
            {
                string alterTableCommand = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnDefinition};";
                using SqliteCommand alterCmd = new SqliteCommand(alterTableCommand, _passwordsDb);
                alterCmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 获取所有的密码列表
        /// </summary>
        /// <returns></returns>
        public static List<PasswordDataModel> GetPasswords(int categoryId = -1)
        {
            List<PasswordDataModel> results = new List<PasswordDataModel>();

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
                item.CategoryId = query.IsDBNull(0) ? -1 : query.GetInt32(1);
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
        /// <param name="thirdPartyLoginId"></param>
        /// <param name="firstLetter"></param>
        /// <param name="name"></param>
        /// <param name="createDate"></param>
        /// <param name="website"></param>
        /// <param name="note"></param>
        /// <param name="favorite"></param>
        /// <param name="logo"></param>
        public static void AddPassword(int categoryid, string account, string password, int thirdPartyLoginId, string firstLetter, string name, string createDate, string website, string note, bool favorite, string logo)
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
            insertCommand.Parameters.AddWithValue("$thirdpartyid", thirdPartyLoginId);
            insertCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新指定ID的密码的信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="categoryid"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="thirdPartyLoginId"></param>
        /// <param name="firstLetter"></param>
        /// <param name="name"></param>
        /// <param name="editDate"></param>
        /// <param name="website"></param>
        /// <param name="note"></param>
        /// <param name="favorite"></param>
        /// <param name="logo"></param>
        public static void UpdatePassword(long id, int categoryid, string account, string password, int thirdPartyLoginId, string firstLetter, string name, string editDate, string website, string note, bool favorite, string logo)
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
            updateCommand.Parameters.AddWithValue("$thirdpartyid", thirdPartyLoginId);
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
