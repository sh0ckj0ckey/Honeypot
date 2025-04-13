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
                    "logo TEXT);";
            SqliteCommand createPasswordsTable = new SqliteCommand(passwordsTableCommand, _passwordsDb);
            using SqliteCommand createPasswordsTable = new SqliteCommand(passwordsTableCommand, _passwordsDb);
            createPasswordsTable.ExecuteNonQuery();
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
        /// <param name="firstLetter"></param>
        /// <param name="name"></param>
        /// <param name="createDate"></param>
        /// <param name="website"></param>
        /// <param name="note"></param>
        /// <param name="favorite"></param>
        /// <param name="logo"></param>
        public static void AddPassword(int categoryid, string account, string password, string firstLetter, string name, string createDate, string website, string note, bool favorite, string logo)
        {
            using SqliteCommand insertCommand = _passwordsDb.CreateCommand();
            insertCommand.CommandText =
            @"
                    INSERT INTO passwords(categoryid,account,password,firstletter,name,createdate,website,note,favorite,logo) 
                    VALUES($categoryid,$account,$password,$firstletter,$name,$createdate,$website,$note,$favorite,$logo);
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
            insertCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新指定ID的密码的信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="categoryid"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="firstLetter"></param>
        /// <param name="name"></param>
        /// <param name="createDate"></param>
        /// <param name="editDate"></param>
        /// <param name="website"></param>
        /// <param name="note"></param>
        /// <param name="favorite"></param>
        /// <param name="logo"></param>
        public static void UpdatePassword(long id, int categoryid, string account, string password, string firstLetter, string name, string editDate, string website, string note, bool favorite, string logo)
        {
            SqliteCommand insertCommand = new SqliteCommand(
                $"UPDATE passwords SET categoryid=$categoryid,account=$account,password=$password,firstletter=$firstletter,name=$name,editdate=$editdate,website=$website,note=$note,favorite=$favorite,logo=$logo WHERE id=$id;",
                _passwordsDb);
            insertCommand.Parameters.AddWithValue("$categoryid", categoryid);
            insertCommand.Parameters.AddWithValue("$account", account);
            insertCommand.Parameters.AddWithValue("$password", password);
            insertCommand.Parameters.AddWithValue("$firstletter", firstLetter);
            insertCommand.Parameters.AddWithValue("$name", name);
            insertCommand.Parameters.AddWithValue("$editdate", editDate);
            insertCommand.Parameters.AddWithValue("$website", website);
            insertCommand.Parameters.AddWithValue("$note", note);
            insertCommand.Parameters.AddWithValue("$favorite", favorite ? 1 : 0);
            insertCommand.Parameters.AddWithValue("$logo", logo);
            insertCommand.Parameters.AddWithValue("$id", id);
            insertCommand?.ExecuteNonQuery();
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
        /// 删除指定ID的密码
        /// </summary>
        /// <param name="id"></param>
        public static void DeletePassword(long id)
        {
            using SqliteCommand deleteCommand = _passwordsDb.CreateCommand();
            deleteCommand.CommandText = @"DELETE FROM passwords WHERE id=$id;";

            deleteCommand.Parameters.AddWithValue("$id", id);
            deleteCommand.ExecuteNonQuery();
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
