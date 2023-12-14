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
using CommunityToolkit.WinUI;
using Honeypot.Helpers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Honeypot.Controls
{
    public sealed partial class CreateCategoryControl : UserControl
    {
        private MainViewModel MainViewModel = null;
        public CreateCategoryControl()
        {
            this.InitializeComponent();

            MainViewModel = MainViewModel.Instance;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.LoadSegoeFluentIcons();
        }

        /// <summary>
        /// 选择了一个图标后，更新图标预览
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectIcon(object sender, SelectionChangedEventArgs e)
        {
            if (sender is GridView gv && gv.SelectedItem is Character character)
            {
                AddingCategoryIconPreview.Glyph = character.Char;
            }
        }

        /// <summary>
        /// 重置页面的方法，将会清空创建分类时的输入，并返回到列表界面
        /// </summary>
        public void ResetView(string title = "", string icon = "\uE72E")
        {
            if (AddingCategoryIconPreview is not null)
            {
                AddingCategoryIconPreview.Glyph = icon;
            }

            if (AddingCategoryNameTextBox is not null)
            {
                AddingCategoryNameTextBox.Text = title;
            }

            if (AddingCategoryIconGridView is not null)
            {
                AddingCategoryIconGridView.SelectedIndex = -1;

                try
                {
                    AddingCategoryIconGridView.SmoothScrollIntoViewWithIndexAsync(0, disableAnimation: true);
                }
                catch { }
            }
        }

        public Tuple<string, string> GetCategoryInfo()
        {
            try
            {
                string title = AddingCategoryNameTextBox?.Text;
                string icon = AddingCategoryIconPreview?.Glyph;

                return Tuple.Create(title, icon);
            }
            catch { }
            return null;
        }

    }
}
