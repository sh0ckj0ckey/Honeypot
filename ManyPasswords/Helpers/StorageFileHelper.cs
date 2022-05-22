using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace ManyPasswords
{
    public class StorageFileHelper
    {
        //存储数据的文件夹名称
        private const string DataFolderName = "Data";

        //存储数据的文件夹对象(单例，见下面的GetDataFolder方法)
        private static IStorageFolder DataFolder = null;

        /// <summary>
        /// 获取存储数据的文件夹的对象
        /// </summary>
        /// <returns></returns>
        public static async Task<IStorageFolder> GetDataFolder()
        {
            if (DataFolder == null)
            {
                DataFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(DataFolderName, CreationCollisionOption.OpenIfExists);
            }
            return DataFolder;
        }

        /// <summary>
        /// 读取本地文件夹根目录的文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<string> ReadFileAsync(string fileName)
        {
            string text = string.Empty;
            try
            {
                IStorageFolder applicationFolder = await GetDataFolder();
                IStorageFile storageFile = await applicationFolder.GetFileAsync(fileName);
                IRandomAccessStream accessStream = await storageFile.OpenReadAsync();
                using (StreamReader streamReader = new StreamReader(accessStream.AsStreamForRead((int)accessStream.Size)))
                {
                    text = streamReader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                //text = "文件读取错误：" + e.Message;
            }
            return text;
        }

        /// <summary>
        /// 写入本地文件夹根目录的文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task<string> WriteFileAsync(string fileName, string content)
        {
            try
            {
                IStorageFolder applicationFolder = await GetDataFolder();
                IStorageFile storageFile = await applicationFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                Int32 retryAttempts = 3;
                const Int32 ERROR_ACCESS_DENIED = unchecked((Int32)0x80070005);
                const Int32 ERROR_SHARING_VIOLATION = unchecked((Int32)0x80070020);

                while (retryAttempts > 0)
                {
                    try
                    {
                        retryAttempts--;
                        await FileIO.WriteTextAsync(storageFile, content);
                        break;
                    }
                    catch (Exception ex) when ((ex.HResult == ERROR_ACCESS_DENIED) || (ex.HResult == ERROR_SHARING_VIOLATION))
                    {
                        await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1));
                    }
                    catch (Exception e) { return "写入失败：" + e.Message; }
                }
                return string.Empty;
            }
            catch (Exception e) { return "写入失败：" + e.Message; }
        }

        /// <summary>
        /// 读取旧版本本地文件夹根目录的密码文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<T> ReadOldFileAsync<T>(string fileName)
        {
            try
            {
                //获取实体类类型实例化一个对象
                T sessionState = default(T);
                IStorageFolder applicationFolder = await GetDataFolder();
                StorageFile file = await applicationFolder.GetFileAsync(fileName);
                if (file != null && File.Exists(file.Path))
                {
                    try
                    {
                        using (IInputStream inStream = await file.OpenSequentialReadAsync())
                        {
                            //反序列化XML
                            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                            sessionState = (T)serializer.ReadObject(inStream.AsStreamForRead());
                        }
                    }
                    catch { }
                }
                return sessionState;
            }
            catch { }
            return default(T);
        }

        public static async Task<string> CreatePasswordsZipFile()
        {
            try
            {
                IStorageFolder sourcefolder = ApplicationData.Current.LocalFolder;

                if (sourcefolder != null)
                {
                    //Windows.Storage.Pickers.FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker();
                    //savePicker.FileTypeChoices.Add("压缩文件", new List<string>() { ".zip" });
                    //savePicker.SuggestedFileName = "ManyPasswords.zip";
                    //Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();

                    var folderPicker = new Windows.Storage.Pickers.FolderPicker();
                    folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
                    folderPicker.FileTypeFilter.Add("*");
                    Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();
                    if (folder != null)
                    {
                        try
                        {
                            // Application now has read/write access to all contents in the picked folder (including other sub-folder contents)
                            Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                        }
                        catch { }

                        string result = await Task<string>.Run(() =>
                        {
                            try
                            {
                                string basePath = folder.Path + "//ManyPasswords";
                                StringBuilder destPathSb = new StringBuilder();
                                while (true)
                                {
                                    destPathSb.Clear();
                                    destPathSb.Append(basePath);
                                    destPathSb.Append(DateTime.Now.Ticks);
                                    destPathSb.Append(".zip");
                                    if (!File.Exists(destPathSb.ToString()))
                                    {
                                        break;
                                    }
                                }

                                ZipFile.CreateFromDirectory(sourcefolder.Path, destPathSb.ToString(), CompressionLevel.Optimal, true);
                            }
                            catch (Exception e)
                            {
                                return e.Message;
                            }
                            return string.Empty;
                        });
                        return result;
                    }
                }
            }
            catch { }
            return string.Empty;
        }
    }
}
