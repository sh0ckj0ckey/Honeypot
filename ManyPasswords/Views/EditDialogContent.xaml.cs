﻿using ManyPasswords.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace ManyPasswords.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class EditDialogContent : Page
    {
        ViewModel.PasswordViewModel ViewModel = null;

        public EditDialogContent(Models.PasswordItem editing)
        {
            try
            {
                ViewModel = PasswordViewModel.Instance;
            }
            catch { }
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //创建和自定义 FileOpenPicker
                var picker = new Windows.Storage.Pickers.FileOpenPicker
                {
                    ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                    SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
                };
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".png");

                //选取单个文件
                var file = await picker.PickSingleFileAsync();

                if (file != null)
                {
                    string desiredName = "password_icon_" + DateTime.Now.Ticks + file.FileType;

                    StorageFolder applicationFolder = ApplicationData.Current.LocalFolder;
                    StorageFolder folder = await applicationFolder.CreateFolderAsync("Pic", CreationCollisionOption.OpenIfExists);

                    try
                    {
                        StorageFile saveFile = await file.CopyAsync(folder, desiredName, NameCollisionOption.GenerateUniqueName);
                        ViewModel.EditingTempPassword.sPicture = saveFile.Path;
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}
