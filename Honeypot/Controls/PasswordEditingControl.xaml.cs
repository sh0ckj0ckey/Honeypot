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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Honeypot.Controls
{
    public sealed partial class PasswordEditingControl : UserControl
    {
        private MainViewModel MainViewModel = null;

        private int _editingId = -1;

        public PasswordEditingControl()
        {
            this.InitializeComponent();

            MainViewModel = MainViewModel.Instance;
        }

        public void SetOriginInfo(int id, string account, string password, string name, string website, string note, int categoryId, string logoFile)
        {
            _editingId = id;

            AccountTextBox.Text = account;
            PasswordTextBox.Text = password;
            NameTextBox.Text = name;
            NameTextBox.PlaceholderText = name;
            WebsiteTextBox.Text = website;
            NoteTextBox.Text = note;
        }

        public int GetModifiedInfo(out string account, out string password, out string name, out string website, out string note, out int categoryId, out string logoFile)
        {
            account = AccountTextBox.Text;
            password = PasswordTextBox.Text;
            name = NameTextBox.Text;
            website = WebsiteTextBox.Text;
            note = NoteTextBox.Text;
            categoryId = 0;
            logoFile = "";

            return _editingId;
        }

        public void ResetView()
        {

        }

        private void OnClickPickFile(object sender, RoutedEventArgs e)
        {

        }
    }
}
