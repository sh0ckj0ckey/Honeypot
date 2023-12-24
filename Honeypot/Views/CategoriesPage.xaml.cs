using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CommunityToolkit.WinUI;
using Honeypot.Controls;
using Honeypot.Helpers;
using Honeypot.Models;
using Honeypot.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Honeypot.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CategoriesPage : Page
    {
        private MainViewModel MainViewModel = null;

        private CreateCategoryControl _createCategoryControl = new CreateCategoryControl();

        private ContentDialog _createCategoryDialog = null;

        private ContentDialog _deleteConfirmDialog = null;

        public CategoriesPage()
        {
            this.InitializeComponent();

            MainViewModel = MainViewModel.Instance;

            _createCategoryDialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "",
                Content = _createCategoryControl,
                PrimaryButtonText = "",
                CloseButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary
            };

            _deleteConfirmDialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "删除确认",
                Content = "确定要删除吗，删除后将无法恢复",
                PrimaryButtonText = "确认",
                CloseButtonText = "取消",
                DefaultButton = ContentDialogButton.Close
            };
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
        /// 点击创建新分类
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickCreateCategory(object sender, RoutedEventArgs e)
        {
            _createCategoryControl.ResetView();
            _createCategoryDialog.Title = "新建分类";
            _createCategoryDialog.PrimaryButtonText = "创建";
            _createCategoryDialog.XamlRoot = this.XamlRoot;
            _createCategoryDialog.RequestedTheme = this.ActualTheme;
            ContentDialogResult result = await _createCategoryDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var newCategoryInfo = _createCategoryControl.GetCategoryInfo();
                MainViewModel.Instance.CreateCategory(newCategoryInfo?.Item1, newCategoryInfo?.Item2);
            }
        }

        /// <summary>
        /// 点击将分类置于最前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickMoveCategory(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem item && item.DataContext is CategoryModel categoty)
            {
                MainViewModel.Instance.MoveCategory(categoty);
            }
        }

        /// <summary>
        /// 点击编辑分类信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickEditCategory(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem item && item.DataContext is CategoryModel categoty)
            {
                _createCategoryControl.ResetView(categoty.Title, categoty.Icon);
                _createCategoryDialog.Title = "编辑分类";
                _createCategoryDialog.PrimaryButtonText = "修改";
                _createCategoryDialog.XamlRoot = this.XamlRoot;
                _createCategoryDialog.RequestedTheme = this.ActualTheme;
                ContentDialogResult result = await _createCategoryDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    var newCategoryInfo = _createCategoryControl.GetCategoryInfo();
                    MainViewModel.Instance.EditCategory(categoty, newCategoryInfo?.Item1, newCategoryInfo?.Item2);
                }
            }
        }

        /// <summary>
        /// 点击删除分类
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickDeleteCategory(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem item && item.DataContext is CategoryModel categoty)
            {
                _deleteConfirmDialog.Content = $"确定要删除分类 \"{categoty.Title}\" 吗？该分类下的密码不会被删除。";
                _deleteConfirmDialog.XamlRoot = this.XamlRoot;
                _deleteConfirmDialog.RequestedTheme = this.ActualTheme;
                ContentDialogResult result = await _deleteConfirmDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    MainViewModel.Instance.DeleteCategory(categoty.Id);
                }
            }
        }
    }
}
