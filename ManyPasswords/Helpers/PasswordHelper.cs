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
        /// <summary>
        /// 保存密码列表
        /// </summary>
        public static async void SaveData(ObservableCollection<Models.PasswordItem> passwords)
        {
            try
            {
                string data = JsonConvert.SerializeObject(passwords);
                string rsadata = RSAHelper.RSAEncrypt(data);
                await StorageFileHelper.WriteAsync(rsadata, "manypasswords.dat");
            }
            catch { }
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
                string data = await StorageFileHelper.ReadAsync<string>("manypasswords.dat");
                if (!string.IsNullOrEmpty(data))
                {
                    data = RSAHelper.RSADecrypt(data);

                    JsonSerializerSettings jss = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    passwordsList = JsonConvert.DeserializeObject<List<Models.PasswordItem>>(data, jss);
                }
            }
            catch (Exception e) { }
            return passwordsList;
        }

    }
}
