using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Helpers;
using Windows.Storage.Streams;
using Windows.Storage;
using System.Diagnostics;

namespace Honeypot.Helpers
{
    public class MigrateHelper
    {
        // 存储数据的文件夹名称
        private const string _dataFolderName = "Data";

        private const string _passwordFileName = "manypasswords.pswd";

        /// <summary>
        /// 读取密码列表
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetLegacyJson()
        {
            string json = string.Empty;
            try
            {
                IStorageFolder applicationFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(_dataFolderName, CreationCollisionOption.OpenIfExists);
                IStorageFile storageFile = await applicationFolder.GetFileAsync(_passwordFileName);
                IRandomAccessStream accessStream = await storageFile.OpenReadAsync();
                using StreamReader streamReader = new(accessStream.AsStreamForRead((int)accessStream.Size));
                json = streamReader.ReadToEnd();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return json;
        }

        public static async void DeleteLegacy()
        {
            try
            {
                IStorageFolder applicationFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(_dataFolderName, CreationCollisionOption.OpenIfExists);
                IStorageFile storageFile = await applicationFolder.GetFileAsync(_passwordFileName);
                await storageFile.DeleteAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
