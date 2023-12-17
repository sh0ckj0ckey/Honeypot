using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Honeypot.Data.Models;
using Microsoft.Data.Sqlite;
using Windows.Storage;
using WinRT;

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
                    "icon TEXT);";
            SqliteCommand createCategoryTable = new SqliteCommand(categoryTableCommand, _passwordsDb);
            createCategoryTable.ExecuteReader();

            string passwordsTableCommand =
                "CREATE TABLE IF NOT EXISTS passwords (" +
                    "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE," +
                    "categoryid INTEGER DEFAULT 0," +
                    "account TEXT," +
                    "password TEXT," +
                    "firstletter TEXT," +
                    "name TEXT," +
                    "createdate TEXT," +
                    "editdate TEXT," +
                    "website TEXT," +
                    "note TEXT," +
                    "favorite INTEGER DEFAULT 0," +
                    "image BLOB);";
            SqliteCommand createGlossaryTable = new SqliteCommand(passwordsTableCommand, _passwordsDb);
            createGlossaryTable.ExecuteReader();
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
        private static byte[] GetBytes(SqliteDataReader reader, int ordinal)
        {
            const int CHUNK_SIZE = 2 * 1024;
            byte[] buffer = new byte[CHUNK_SIZE];
            long bytesRead;
            long fieldOffset = 0;
            using MemoryStream stream = new();
            while ((bytesRead = reader.GetBytes(ordinal, fieldOffset, buffer, 0, buffer.Length)) > 0)
            {
                stream.Write(buffer, 0, (int)bytesRead);
                fieldOffset += bytesRead;
            }
            return stream.ToArray();
        }

        /// <summary>
        /// 获取所有的密码列表
        /// </summary>
        /// <returns></returns>
        public static List<PasswordDataModel> GetPasswords(int categoryId = -1)
        {
            try
            {
                List<PasswordDataModel> results = new List<PasswordDataModel>();
                SqliteCommand selectCommand = null;

                if (categoryId >= 0)
                {
                    selectCommand = new($"SELECT * FROM passwords WHERE categoryid=$categoryid", _passwordsDb);
                    selectCommand.Parameters.AddWithValue("$categoryid", categoryId);
                }
                else
                {
                    selectCommand = new($"SELECT * FROM passwords", _passwordsDb);
                }

                SqliteDataReader query = selectCommand?.ExecuteReader();
                while (query?.Read() == true)
                {
                    PasswordDataModel item = new PasswordDataModel();
                    item.Id = query.IsDBNull(0) ? -1 : query.GetInt32(0);
                    item.Account = query.IsDBNull(2) ? string.Empty : query.GetString(2);
                    item.Password = query.IsDBNull(3) ? string.Empty : query.GetString(3);
                    item.FirstLetter = query.IsDBNull(4) ? string.Empty : query.GetString(4);
                    item.Name = query.IsDBNull(5) ? string.Empty : query.GetString(5);
                    item.CreateDate = query.IsDBNull(6) ? string.Empty : query.GetString(6);
                    item.EditDate = query.IsDBNull(7) ? string.Empty : query.GetString(7);
                    item.Website = query.IsDBNull(8) ? string.Empty : query.GetString(8);
                    item.Note = query.IsDBNull(9) ? string.Empty : query.GetString(9);
                    item.Favorite = query.IsDBNull(10) ? 0 : query.GetInt32(10);
                    item.Image = query.IsDBNull(11) ? null : GetBytes(query, 11);
                    results.Add(item);
                }
                return results;
            }
            catch { }
            return null;
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
        /// <param name="editDate"></param>
        /// <param name="website"></param>
        /// <param name="note"></param>
        /// <param name="favorite"></param>
        /// <param name="image"></param>
        public static void AddPassword(int categoryid, string account, string password, string firstLetter, string name, string createDate, string editDate, string website, string note, bool favorite, byte[] image)
        {
            try
            {
                SqliteCommand insertCommand = new SqliteCommand(
                    $"INSERT INTO passwords(categoryid,account,password,firstletter,name,createdate,editdate,website,note,favorite,image) VALUES($categoryid,$account,$password,$firstletter,$name,$createdate,$editdate,$website,$note,$favorite,$image);",
                    _passwordsDb);
                insertCommand.Parameters.AddWithValue("$categoryid", categoryid);
                insertCommand.Parameters.AddWithValue("$account", account);
                insertCommand.Parameters.AddWithValue("$password", password);
                insertCommand.Parameters.AddWithValue("$firstletter", firstLetter);
                insertCommand.Parameters.AddWithValue("$name", name);
                insertCommand.Parameters.AddWithValue("$createdate", createDate);
                insertCommand.Parameters.AddWithValue("$editdate", editDate);
                insertCommand.Parameters.AddWithValue("$website", website);
                insertCommand.Parameters.AddWithValue("$note", note);
                insertCommand.Parameters.AddWithValue("$favorite", favorite ? 1 : 0);
                insertCommand.Parameters.AddWithValue("$image", image);
                SqliteDataReader query = insertCommand?.ExecuteReader();
            }
            catch { }
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
        /// <param name="image"></param>
        public static void UpdatePassword(long id, int categoryid, string account, string password, string firstLetter, string name, string editDate, string website, string note, bool favorite, byte[] image)
        {
            try
            {
                SqliteCommand insertCommand = new SqliteCommand(
                    $"UPDATE passwords SET categoryid=$categoryid,account=$account,password=$password,firstletter=$firstletter,name=$name,createdate=$createdate,editdate=$editdate,website=$website,note=$note,favorite=$favorite,image=$image WHERE id=$id;",
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
                insertCommand.Parameters.AddWithValue("$image", image);
                insertCommand.Parameters.AddWithValue("$id", id);
                SqliteDataReader query = insertCommand?.ExecuteReader();
            }
            catch { }
        }

        /// <summary>
        /// 删除指定ID的密码
        /// </summary>
        /// <param name="id"></param>
        public static void DeletePassword(long id)
        {
            try
            {
                SqliteCommand deleteWordsCommand = new SqliteCommand($"DELETE FROM passwords WHERE id=$id;", _passwordsDb);
                deleteWordsCommand.Parameters.AddWithValue("$id", id);
                SqliteDataReader wordsQuery = deleteWordsCommand?.ExecuteReader();
            }
            catch { }
        }

        /// <summary>
        /// 获取所有的密码分类列表
        /// </summary>
        /// <returns></returns>
        public static List<CategoryDataModel> GetCategories()
        {
            try
            {
                List<CategoryDataModel> results = new List<CategoryDataModel>();
                SqliteCommand selectCommand = new SqliteCommand($"SELECT * FROM passwordCategories", _passwordsDb);
                SqliteDataReader query = selectCommand?.ExecuteReader();
                while (query?.Read() == true)
                {
                    CategoryDataModel item = new CategoryDataModel();
                    item.Id = query.IsDBNull(0) ? -1 : query.GetInt32(0);
                    item.Title = query.IsDBNull(1) ? string.Empty : query.GetString(1);
                    item.Icon = query.IsDBNull(2) ? string.Empty : query.GetString(2);
                    results.Add(item);
                }
                return results;
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 添加一个分类
        /// </summary>
        /// <param name="title"></param>
        /// <param name="icon"></param>
        public static void AddOneCategory(string title, string icon)
        {
            try
            {
                SqliteCommand insertCommand = new SqliteCommand($"INSERT INTO passwordCategories(title, icon) VALUES($title, $icon);", _passwordsDb);
                insertCommand.Parameters.AddWithValue("$title", title);
                insertCommand.Parameters.AddWithValue("$icon", icon);
                SqliteDataReader query = insertCommand?.ExecuteReader();
            }
            catch { }
        }

        /// <summary>
        /// 修改分类的属性
        /// </summary>
        /// <param name="title"></param>
        /// <param name="icon"></param>
        public static void UpdateOneCategory(int id, string title, string icon)
        {
            try
            {
                SqliteCommand updateCommand = new SqliteCommand($"UPDATE passwordCategories SET title=$title, icon=$icon WHERE id=$id;", _passwordsDb);
                updateCommand.Parameters.AddWithValue("$title", title);
                updateCommand.Parameters.AddWithValue("$icon", icon);
                updateCommand.Parameters.AddWithValue("$id", id);
                SqliteDataReader query = updateCommand?.ExecuteReader();
            }
            catch { }
        }

        /// <summary>
        /// 一个分类，并将passwords表中所有属于该分类的密码分类改为空
        /// </summary>
        /// <param name="id"></param>
        public static void DeleteOneCategory(int id)
        {
            try
            {
                SqliteCommand deleteCommand = new SqliteCommand($"DELETE FROM passwordCategories WHERE id=$id;", _passwordsDb);
                deleteCommand.Parameters.AddWithValue("$id", id);
                SqliteDataReader query = deleteCommand?.ExecuteReader();

                SqliteCommand deleteWordsCommand = new SqliteCommand($"UPDATE passwords SET categoryid=-1 WHERE categoryid=$id;", _passwordsDb);
                deleteWordsCommand.Parameters.AddWithValue("$id", id);
                SqliteDataReader wordsQuery = deleteWordsCommand?.ExecuteReader();
            }
            catch { }
        }

        ///// <summary>
        ///// 获取密码数量
        ///// </summary>
        ///// <param name="categoryId"></param>
        ///// <returns></returns>
        //public static int GetPasswordsCount(int categoryId = -1)
        //{
        //    try
        //    {
        //        SqliteCommand selectCommand = null;
        //        if (categoryId >= 0)
        //        {
        //            selectCommand = new SqliteCommand($"select count(*) from passwords where categoryid=$categoryid", _passwordsDb);
        //            selectCommand.Parameters.AddWithValue("$categoryid", categoryId);
        //        }
        //        else
        //        {
        //            selectCommand = new SqliteCommand($"select count(*) from passwords", _passwordsDb);
        //        }

        //        SqliteDataReader query = selectCommand?.ExecuteReader();
        //        while (query?.Read() == true)
        //        {
        //            var count = query.IsDBNull(0) ? -1 : query.GetInt32(0);
        //            return count;
        //        }
        //    }
        //    catch { }
        //    return -1;
        //}
    }
}
