using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CommunityToolkit.WinUI;
using ManyPasswords3.Helpers;
using ManyPasswords3.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ManyPasswords3.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CategoriesPage : Page
    {
        private MainViewModel MainViewModel = null;

        public CategoriesPage()
        {
            this.InitializeComponent();

            MainViewModel = MainViewModel.Instance;

            MainViewModel.Instance.LoadSegoeFluentIcons();
        }

        /// <summary>
        /// 每次加载页面后都重置页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ResetPage();
        }

        /// <summary>
        /// 点击查看指定分类下的密码列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickCategory(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 点击确认，添加新的分类，并重置页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickCreate(object sender, RoutedEventArgs e)
        {
            string title = AddingCategoryNameTextBox?.Text;
            string icon = AddingCategoryIconPreview?.Glyph;

            MainViewModel.Instance.CreateCategory(title, icon);

            ResetPage();
        }

        /// <summary>
        /// 点击取消，重置页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickCancel(object sender, RoutedEventArgs e)
        {
            ResetPage();
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
        private void ResetPage()
        {
            if (CreateToggleButton is not null)
            {
                CreateToggleButton.IsChecked = false;
            }

            if (AddingCategoryIconPreview is not null)
            {
                AddingCategoryIconPreview.Glyph = "";
            }

            if (AddingCategoryNameTextBox is not null)
            {
                AddingCategoryNameTextBox.Text = "";
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
    }
}
