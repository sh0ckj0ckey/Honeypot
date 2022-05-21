using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManyPasswords.Helpers;
using System.Collections.ObjectModel;

namespace ManyPasswords
{
    public class PasswordHelper
    {
        private const string _passwordFileName = "manypasswords.pswd";

        /// <summary>
        /// 保存密码列表
        /// </summary>
        public static async Task<string> SaveData(List<Models.PasswordItem> passwords)
        {
            try
            {
                string data = JsonConvert.SerializeObject(passwords);
                string msg = await StorageFileHelper.WriteFileAsync(_passwordFileName, data);
                return msg;
            }
            catch (Exception e) { return "保存失败：" + e.Message; }
        }

        /// <summary>
        /// 读取密码列表
        /// </summary>
        /// <returns></returns>
        public static async Task<List<Models.PasswordItem>> LoadData()
        {
            List<Models.PasswordItem> passwordsList = new List<Models.PasswordItem>();

            try
            {
                string data = await StorageFileHelper.ReadFileAsync(_passwordFileName);
                if (!string.IsNullOrEmpty(data))
                {
                    JsonSerializerSettings jss = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    passwordsList = JsonConvert.DeserializeObject<List<Models.PasswordItem>>(data, jss);
                }
            }
            catch { }

            try
            {
                // 读取旧版本密码
                var oldList = await LoadOldData();
                if (oldList != null && oldList.Count > 0)
                {
                    passwordsList.AddRange(oldList);
                    _ = await SaveData(passwordsList);
                }
            }
            catch { }

            return passwordsList;
        }

        public static async Task<List<Models.PasswordItem>> LoadOldData()
        {
            try
            {
                Windows.Storage.IStorageFolder applicationFolder = await StorageFileHelper.GetDataFolder();
                string oldFilePath = applicationFolder.Path + "\\Password.dat";
                if (!File.Exists(oldFilePath))
                {
                    return null;
                }

                List<好多密码_UWP.OnePassword> passwordsList = null;

                string oldPasswordString = await StorageFileHelper.ReadOldFileAsync<string>("Password.dat");
                if (!string.IsNullOrEmpty(oldPasswordString))
                {
                    try
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.MissingMemberHandling = MissingMemberHandling.Ignore;
                        serializer.NullValueHandling = NullValueHandling.Ignore;
                        StringReader sr = new StringReader(oldPasswordString);

                        object o = serializer.Deserialize(new JsonTextReader(sr), typeof(List<好多密码_UWP.OnePassword>));
                        List<好多密码_UWP.OnePassword> list = o as List<好多密码_UWP.OnePassword>;
                        passwordsList = list;
                    }
                    catch { }
                }

                if (passwordsList == null || passwordsList.Count <= 0)
                {
                    try
                    {
                        var list = await StorageFileHelper.ReadOldFileAsync<List<好多密码_UWP.OnePassword>>("Password.dat");
                        passwordsList = list;
                    }
                    catch { }
                }

                var newList = ConvertOldPasswordItems(passwordsList);

                if (newList != null && File.Exists(oldFilePath))
                {
                    File.Move(oldFilePath, applicationFolder.Path + "\\OldPasswordBackup.dat");
                    return newList;
                }
            }
            catch { }
            return null;
        }

        private static List<Models.PasswordItem> ConvertOldPasswordItems(List<好多密码_UWP.OnePassword> list)
        {
            try
            {
                if (list != null && list.Count > 0)
                {
                    List<Models.PasswordItem> newPasswordsList = new List<Models.PasswordItem>();
                    foreach (var password in list)
                    {
                        var item = new Models.PasswordItem();
                        item.sAccount = password.Account;
                        item.sPassword = password.Password;
                        item.sFirstLetter = password.FirstLetter;
                        item.sName = password.Name;
                        item.sDate = password.Date;
                        item.sPicture = password.Picture;
                        item.sWebsite = password.Website;
                        item.sNote = password.Info;
                        item.bFavorite = password.IsFavorite;
                        newPasswordsList.Add(item);
                    }
                    return newPasswordsList;
                }
            }
            catch { return null; }
            return new List<Models.PasswordItem>();
        }
    }
}
