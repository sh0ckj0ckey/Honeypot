using System;
using System.Diagnostics;
using Honeypot.Controls;
using Honeypot.Models;
using Honeypot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

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

            var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();

            _createCategoryDialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "",
                Content = _createCategoryControl,
                PrimaryButtonText = "",
                CloseButtonText = resourceLoader.GetString("DialogButtonCancel"),
                DefaultButton = ContentDialogButton.Primary
            };

            _deleteConfirmDialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = resourceLoader.GetString("DialogTitleConfirmDelete"),
                Content = resourceLoader.GetString("DialogContentConfirmDelete"),
                PrimaryButtonText = resourceLoader.GetString("DialogButtonConfirm"),
                CloseButtonText = resourceLoader.GetString("DialogButtonCancel"),
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
            if (sender is Button btn && btn.DataContext is CategoryModel category)
            {
                MainViewModel.Instance.UpdatePasswords(category.Id);
                MainViewModel.Instance.ActNavigatePage?.Invoke(NavigatePageEnum.Passwords);
            }
        }

        /// <summary>
        /// 点击创建新分类
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickCreateCategory(object sender, RoutedEventArgs e)
        {
            var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
            _createCategoryControl.ResetView();
            _createCategoryDialog.Title = resourceLoader.GetString("DialogTitleNewCategory");
            _createCategoryDialog.PrimaryButtonText = resourceLoader.GetString("DialogButtonNewCategory");
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
                var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
                _createCategoryControl.ResetView(categoty.Title, categoty.Icon);
                _createCategoryDialog.Title = resourceLoader.GetString("DialogTitleEditCategory");
                _createCategoryDialog.PrimaryButtonText = resourceLoader.GetString("DialogButtonEditCategory");
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
                var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
                _deleteConfirmDialog.Content = $"{resourceLoader.GetString("DialogContentDeleteCategory1")} \"{categoty.Title}\" {resourceLoader.GetString("DialogContentDeleteCategory2")}";
                _deleteConfirmDialog.XamlRoot = this.XamlRoot;
                _deleteConfirmDialog.RequestedTheme = this.ActualTheme;
                ContentDialogResult result = await _deleteConfirmDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    MainViewModel.Instance.DeleteCategory(categoty.Id);
                }
            }
        }

        /// <summary>
        /// 鼠标移入，显示更多选项按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn)
                {
                    var button = btn.FindName("OptionButton");
                    if (button is Button optionButton)
                    {
                        optionButton.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 鼠标移走，隐藏更多选项按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn)
                {
                    var button = btn.FindName("OptionButton");
                    if (button is Button optionButton)
                    {
                        optionButton.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
