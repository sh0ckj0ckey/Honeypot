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
            return passwordsList;
        }

        public static async Task<List<Models.PasswordItem>> LoadOldData()
        {
            try
            {
                List<Models.OnePassword> passwordsList = null;

                // 如果文件存在则读取，读取后把文件名字修改掉但不要删掉，备用以防万一
                string load = await StorageFileHelper.ReadOldFileAsync<string>("Password.dat");
                if (string.IsNullOrEmpty(load))
                {
                    var ls = await StorageFileHelper.ReadOldFileAsync<List<Models.OnePassword>>("Password.dat");
                    if (ls != null || ls != default(List<Models.OnePassword>))
                    {
                        passwordsList = ls;
                    }
                }
                else
                {
                    JsonSerializer serializer = new JsonSerializer();
                    StringReader sr = new StringReader(load);
                    object o = serializer.Deserialize(new JsonTextReader(sr), typeof(List<Models.OnePassword>));
                    List<Models.OnePassword> list = o as List<Models.OnePassword>;

                    PasswordHelper._data = list;

                    return (PasswordHelper._data != null);
                }
            }
            catch { }
            return null;
        }

        private static List<Models.PasswordItem> ConvertOldPasswordItems(List<Models.OnePassword> list)
        {
            try
            {

            }
            catch { }
        }
    }
}
