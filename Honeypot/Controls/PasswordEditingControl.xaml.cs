using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Honeypot.ViewModels;
using Honeypot.Models;
using System.Xml.Linq;
using System.Diagnostics;
using CommunityToolkit.WinUI.Controls;
using Honeypot.Helpers;
using Windows.Storage.Pickers;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Honeypot.Controls
{
    public sealed partial class PasswordEditingControl : UserControl
    {
        private MainViewModel MainViewModel = null;

        // ��ǰ���ڱ༭�������ID
        private int _editingId = -1;

        // ָʾ�Ƿ�ѡ����µ�ͼƬ�����ûѡ��������Ҳü��ؼ���ΧΪ100%��˵��ͼƬû�иı�
        private bool _changedLogoImage = false;

        public PasswordEditingControl()
        {
            this.InitializeComponent();

            MainViewModel = MainViewModel.Instance;
        }

        /// <summary>
        /// ���ó�ʼ��Ϣ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="name"></param>
        /// <param name="website"></param>
        /// <param name="note"></param>
        /// <param name="categoryId"></param>
        /// <param name="logoFile"></param>
        public async void SetOriginInfo(int id, string account, string password, string name, string website, string note, int categoryId, string logoFile)
        {
            _editingId = id;
            _changedLogoImage = false;

            NameTextBox.Text = name;
            NameTextBox.PlaceholderText = name;

            AccountTextBox.Text = account;
            PasswordTextBox.Text = password;
            WebsiteTextBox.Text = website;
            NoteTextBox.Text = note;

            if (categoryId > 0)
            {
                foreach (var item in MainViewModel.Instance.Categoryies)
                {
                    if (item.Id == categoryId)
                    {
                        CategoryComboBox.SelectedItem = item;
                        break;
                    }
                }
            }

            var imageLogoFile = await LogoImageHelper.GetLogoImageFile(logoFile);
            await LogoImageCropper.LoadImageFromFile(imageLogoFile);
        }

        /// <summary>
        /// ȡ�ñ༭֮�����Ϣ
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="name"></param>
        /// <param name="website"></param>
        /// <param name="note"></param>
        /// <param name="categoryId"></param>
        /// <param name="logoFile"></param>
        /// <returns></returns>
        public int GetModifiedInfo(out string account, out string password, out string name, out string website, out string note, out int categoryId, out string logoFile)
        {
            name = "";
            account = "";
            password = "";
            website = "";
            note = "";
            categoryId = -1;
            logoFile = "";

            try
            {
                name = NameTextBox.Text;
                account = AccountTextBox.Text;
                password = PasswordTextBox.Text;
                website = WebsiteTextBox.Text;
                note = NoteTextBox.Text;

                if (CategoryComboBox?.SelectedItem is CategoryModel category && category is not null)
                {
                    categoryId = category.Id;
                }

                logoFile = "";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return _editingId;
        }

        /// <summary>
        /// ���ÿؼ���״̬
        /// </summary>
        public void ResetView()
        {
            _editingId = -1;
            _changedLogoImage = false;

            NameTextBox.Text = "";
            NameTextBox.PlaceholderText = "";
            AccountTextBox.Text = "";
            PasswordTextBox.Text = "";
            WebsiteTextBox.Text = "";
            NoteTextBox.Text = "";
            CategoryComboBox.SelectedIndex = -1;

            LogoImageCropper.Source = null;
            LogoImageCropper.Reset();
        }

        /// <summary>
        /// ѡ���µ�ͼ���ļ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickPickFile(object sender, RoutedEventArgs e)
        {
            try
            {
                var filePicker = new FileOpenPicker
                {
                    ViewMode = PickerViewMode.Thumbnail,
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                    FileTypeFilter = { ".png", ".jpg", ".jpeg" }
                };

                WinRT.Interop.InitializeWithWindow.Initialize(filePicker, App.MainWindow.GetWindowHandle());

                var file = await filePicker.PickSingleFileAsync();
                if (file != null && LogoImageCropper != null)
                {
                    await LogoImageCropper.LoadImageFromFile(file);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// ȡ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickRemoveCategory(object sender, RoutedEventArgs e)
        {
            CategoryComboBox.SelectedIndex = -1;
        }
    }
}
