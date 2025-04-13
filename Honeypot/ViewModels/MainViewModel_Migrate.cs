using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Honeypot.Core;
using Honeypot.Helpers;
using Honeypot.Models;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Honeypot.ViewModels
{
    public partial class MainViewModel
    {
        /// <summary>
        /// 弹出或关闭迁移工具
        /// </summary>
        public Action<bool> ActInvokeMigrater { get; set; } = null;

        // 迁移界面的状态
        private MigrateStepEnum _migrateState = MigrateStepEnum.Welcome;
        public MigrateStepEnum MigrateState
        {
            get => _migrateState;
            set => SetProperty(ref _migrateState, value);
        }

        // 是否存在好多密码的旧数据，用于在设置页面控制是否展示迁移工具
        private bool _showMigrater = false;
        public bool ShowMigrater
        {
            get => _showMigrater;
            set => SetProperty(ref _showMigrater, value);
        }

        // 迁移中的文字提示
        private string _migratingHint = string.Empty;
        public string MigratingHint
        {
            get => _migratingHint;
            set => SetProperty(ref _migratingHint, value);
        }

        // 总共等待迁移的密码个数
        private int _countToMigrate = 0;
        public int CountToMigrate
        {
            get => _countToMigrate;
            set => SetProperty(ref _countToMigrate, value);
        }

        // 已经迁移的密码个数
        private int _countMigrated = 0;
        public int CountMigrated
        {
            get => _countMigrated;
            set => SetProperty(ref _countMigrated, value);
        }

        // 迁移错误记录
        private string _migrateFailedContent = string.Empty;
        public string MigrateFailedContent
        {
            get => _migrateFailedContent;
            set => SetProperty(ref _migrateFailedContent, value);
        }

        /// <summary>
        /// 判断是否需要迁移数据
        /// </summary>
        /// <returns></returns>
        public async Task CheckShouldMigrate()
        {
            try
            {
                var json = await MigrateHelper.GetLegacyJson();
                if (!string.IsNullOrWhiteSpace(json))
                {
                var passwords = System.Text.Json.JsonSerializer.Deserialize<List<Models.LegacyPasswordItem>>(json);
                ShowMigrater = passwords.Count > 0;
            }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 迁移数据
        /// </summary>
        public async void MigratePasswords()
        {
            try
            {
                // 准备迁移
                CountToMigrate = 0;
                CountMigrated = 0;
                MigratingHint = string.Empty;
                MigrateState = MigrateStepEnum.BeforeMigrate;
                MigrateFailedContent = string.Empty;
                //MigratingHint = "开始迁移";

                // 读取文件
                //await Task.Delay(800);
                MigrateState = MigrateStepEnum.ReadingFile;
                MigratingHint = "正在读取文件";
                List<LegacyPasswordItem> passwords = null;
                string json = await MigrateHelper.GetLegacyJson();
                await StorageFilesService.WriteFileAsync("legacy.pswd", json);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    passwords = System.Text.Json.JsonSerializer.Deserialize<List<Models.LegacyPasswordItem>>(json);
                }

                // 开始迁移
                await Task.Delay(800);
                MigrateState = MigrateStepEnum.Migrating;
                MigratingHint = "正在写入";
                int failedCount = 0;
                StringBuilder failedContent = new StringBuilder();
                if (passwords?.Count > 0)
                {
                    CountToMigrate = passwords.Count;
                    foreach (var item in passwords)
                    {
                        try
                        {
                            MigratingHint = $"正在写入\"{item?.sName}\" ({CountMigrated + 1}/{CountToMigrate})";

                            // 拷贝图片
                            string logoFilePath = DateTime.Now.Ticks.ToString();
                            WriteableBitmap logoWriteableBitmap = null;
                            bool isBuiltInLogo = item.sPicture.Contains("ms-appx");
                            if (isBuiltInLogo || File.Exists(item.sPicture))
                            {
                                try
                                {
                                    var logoImage = isBuiltInLogo ?
                                                             await StorageFile.GetFileFromApplicationUriAsync(new Uri(item.sPicture)) :
                                                             await StorageFile.GetFileFromPathAsync(item.sPicture);
                                    using IRandomAccessStream stream = await logoImage.OpenAsync(FileAccessMode.Read);
                                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                                    logoWriteableBitmap = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                                    logoWriteableBitmap.SetSource(stream);

                                    bool result = await LogoImageHelper.SaveLogoImage(logoFilePath, logoWriteableBitmap);
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(ex.Message);
                                }
                            }

                            MainViewModel.Instance.AddPassword(
                                -1,
                                item.sAccount,
                                item.sPassword,
                                item.sName,
                                item.sWebsite,
                                item.sNote,
                                item.bFavorite,
                                logoFilePath,
                                item.sDate);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);

                            failedContent.Append("名称: ");
                            failedContent.Append(item?.sName);
                            failedContent.Append("\r\n");
                            failedContent.Append("账号: ");
                            failedContent.Append(item?.sAccount);
                            failedContent.Append("\r\n");
                            failedContent.Append("密码: ");
                            failedContent.Append(item?.sPassword);
                            failedContent.Append("\r\n");
                            failedContent.Append("网址: ");
                            failedContent.Append(item?.sWebsite);
                            failedContent.Append("\r\n");
                            failedContent.Append("备注: ");
                            failedContent.Append(item?.sNote);
                            failedContent.Append("\r\n");
                            failedContent.Append("\r\n");

                            failedCount++;
                        }

                        await Task.Delay(600);
                        CountMigrated++;
                    }
                }
                else
                {
                    CountToMigrate = 1;
                    CountMigrated = 1;
                }

                // 迁移结束
                if (failedCount <= 0)
                {
                    MigratingHint = "迁移完成，欢迎使用密罐";
                    MigrateState = MigrateStepEnum.Successful;
                }
                else
                {
                    MigratingHint = $"迁移结束，下列{failedCount}个账号没能成功添加，请复制这些文本自行处理 :(";
                    MigrateFailedContent = failedContent.ToString();
                    MigrateState = MigrateStepEnum.Failed;
                }

                MigrateHelper.DeleteLegacy();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                ShowTipsContentDialog("糟糕...", $"迁移数据时出现了致命错误：{ex.Message}");
            }
        }
    }
}
