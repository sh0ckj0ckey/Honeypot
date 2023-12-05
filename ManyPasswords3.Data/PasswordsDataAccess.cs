using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManyPasswords3.Data.Models;
using Microsoft.Data.Sqlite;
using Windows.Storage;

namespace ManyPasswords3.Data
{
    public static class PasswordsDataAccess
    {
        private static SqliteConnection _passwordsDb = null;

        public static async void LoadDatabase(StorageFolder folder/*string filePath*/)
        {
            var file = await folder.CreateFileAsync("manypasswords.db", CreationCollisionOption.OpenIfExists);

            string dbpath = file.Path;
            _passwordsDb = new SqliteConnection($"Filename={dbpath}");
            _passwordsDb.Open();

            string categoryTableCommand =
                "CREATE TABLE IF NOT EXISTS passwordCategory (" +
                    "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE," +
                    "title TEXT," +
                    "icon TEXT);";
            SqliteCommand createCategoryTable = new SqliteCommand(categoryTableCommand, _passwordsDb);
            createCategoryTable.ExecuteReader();

            string passwordsTableCommand =
                "CREATE TABLE IF NOT EXISTS passwords (" +
                    "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE," +
                    "account TEXT," +
                    "passwords TEXT," +
                    "firstletter CHAR(4)," +
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

        public static void CloseDatabase()
        {
            _passwordsDb?.Close();
            _passwordsDb?.Dispose();
            _passwordsDb = null;
        }

        /// <summary>
        /// 获取所有的密码分类列表
        /// </summary>
        /// <returns></returns>
        public static List<CategoryDataModel> GetAllCategories()
        {
            try
            {
                List<CategoryDataModel> results = new List<CategoryDataModel>();
                SqliteCommand selectCommand = new SqliteCommand($"SELECT * FROM passwordCategory", _passwordsDb);
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
        /// 添加一个生词本
        /// </summary>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        public static void AddOneGlossary(string title, string desc)
        {
            try
            {
                SqliteCommand insertCommand = new SqliteCommand($"INSERT INTO glossaryCategory(title, description) VALUES($title, $desc);", _passwordsDb);
                insertCommand.Parameters.AddWithValue("$title", title);
                insertCommand.Parameters.AddWithValue("$desc", desc);
                SqliteDataReader query = insertCommand?.ExecuteReader();
            }
            catch { }
        }

        /// <summary>
        /// 修改生词本的属性
        /// </summary>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        public static void UpdateOneGlossary(int id, string title, string desc)
        {
            try
            {
                SqliteCommand updateCommand = new SqliteCommand($"UPDATE glossaryCategory SET title=$title, description=$desc WHERE id=$glossaryid;", _passwordsDb);
                updateCommand.Parameters.AddWithValue("$title", title);
                updateCommand.Parameters.AddWithValue("$desc", desc);
                updateCommand.Parameters.AddWithValue("$glossaryid", id);
                SqliteDataReader query = updateCommand?.ExecuteReader();
            }
            catch { }
        }

        /// <summary>
        /// 删除一个生词本，并删除glossary表中所有关联的生词
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        public static void DeleteOneGlossary(int id)
        {
            try
            {
                SqliteCommand deleteCommand = new SqliteCommand($"DELETE FROM glossaryCategory WHERE id=$glossaryid;", _passwordsDb);
                deleteCommand.Parameters.AddWithValue("$glossaryid", id);
                SqliteDataReader query = deleteCommand?.ExecuteReader();

                SqliteCommand deleteWordsCommand = new SqliteCommand($"DELETE FROM glossary WHERE glossaryid=$glossaryid;", _passwordsDb);
                deleteWordsCommand.Parameters.AddWithValue("$glossaryid", id);
                SqliteDataReader wordsQuery = deleteWordsCommand?.ExecuteReader();
            }
            catch { }
        }

        /// <summary>
        /// 获取指定ID的生词本内生词个数
        /// </summary>
        /// <param name="glossaryId"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static int GetGlossaryWordsCount(int glossaryId, GlossaryColorsEnum color)
        {
            try
            {
                SqliteCommand selectCommand = null;
                if (color != GlossaryColorsEnum.Transparent)
                {
                    selectCommand = new SqliteCommand($"select count(*) from glossary where glossaryid=$glossaryid AND color=$color", _passwordsDb);
                    selectCommand.Parameters.AddWithValue("$glossaryid", glossaryId);
                    selectCommand.Parameters.AddWithValue("$color", (int)color);
                }
                else
                {
                    selectCommand = new SqliteCommand($"select count(*) from glossary where glossaryid=$glossaryid", _passwordsDb);
                    selectCommand.Parameters.AddWithValue("$glossaryid", glossaryId);
                }

                SqliteDataReader query = selectCommand?.ExecuteReader();
                while (query?.Read() == true)
                {
                    StarDictWordItem item = new StarDictWordItem();
                    var count = query.IsDBNull(0) ? -1 : query.GetInt32(0);
                    return count;
                }
            }
            catch { }
            return -1;
        }

    }
}
