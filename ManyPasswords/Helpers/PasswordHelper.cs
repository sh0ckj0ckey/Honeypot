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
    }
}
