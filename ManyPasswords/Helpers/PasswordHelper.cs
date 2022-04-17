using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 好多密码_UWP.Helpers;

namespace 好多密码_UWP
{
    public class PasswordHelper
    {
        public static List<OnePassword> _data = null;

        /// <summary>
        /// 保存密码列表
        /// </summary>
        public static async void SaveData()
        {
            string data = JsonConvert.SerializeObject(_data);
            string rsadata = RSAHelper.RSAEncrypt(data);
            await StorageFileHelper.WriteAsync(rsadata, "Password.dat");
        }

        /// <summary>
        /// 获取已经保存的密码数据，加载到_data中
        /// </summary>
        /// <returns></returns>
        public static async Task<List<OnePassword>> GetData()
        {
            if (_data == null)
            {
                bool isExist = await LoadFromFile();
                if (!isExist)
                {
                    _data = new List<OnePassword>();
                }
            }
            return _data;
        }

        /// <summary>
        /// 读取密码列表
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> LoadFromFile()
        {
            string load = await StorageFileHelper.ReadAsync<string>("Password.dat");
            if (load == null)
            {
                PasswordHelper._data = await StorageFileHelper.ReadAsync<List<OnePassword>>("Password.dat");
                return (PasswordHelper._data != null);
            }

            load = RSAHelper.RSADecrypt(load);
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(load);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(List<OnePassword>));
            List<OnePassword> list = o as List<OnePassword>;

            PasswordHelper._data = list;

            return (PasswordHelper._data != null);
        }

    }
}
