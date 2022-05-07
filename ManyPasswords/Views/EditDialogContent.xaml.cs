using ManyPasswords.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class EditDialogContent : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        ViewModel.PasswordViewModel ViewModel = null;

        // 临时存储编辑信息
        private Models.PasswordItem _editTemp = new Models.PasswordItem(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, false);

        public EditDialogContent(Models.PasswordItem editing)
        {
            try
            {
                ViewModel = PasswordViewModel.Instance;

                _editTemp.sName = editing.sName;
                _editTemp.sAccount = editing.sAccount;
                _editTemp.sPassword = editing.sPassword;
                _editTemp.sPicture = editing.sPicture;
                _editTemp.sWebsite = editing.sWebsite;
                _editTemp.sNote = editing.sNote;
                _editTemp.bFavorite = editing.bFavorite;
            }
            catch { }
            this.InitializeComponent();
        }
    }
}
