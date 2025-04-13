using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Honeypot.Data;
using Honeypot.Helpers;
using Honeypot.Models;

namespace Honeypot.ViewModels
{
    public partial class MainViewModel
    {
        /// <summary>
        /// 所有分类ID与分类信息的映射
        /// </summary>
        private Dictionary<int, CategoryModel> _categoriesDict = new Dictionary<int, CategoryModel>();

        /// <summary>
        /// 所有分类
        /// </summary>
        public ObservableCollection<CategoryModel> Categoryies { get; set; } = new ObservableCollection<CategoryModel>();

        /// <summary>
        /// 显示在侧边导航栏的分类列表，与Categories同步变更
        /// </summary>
        public ObservableCollection<MainNavigationItem> CategoriesOnNav { get; set; } = new ObservableCollection<MainNavigationItem>();

        #region 分类图标列表

        /// <summary>
        /// 本机 Segoe Fluent Icons 字体的所有字符
        /// </summary>
        private ObservableCollection<Character> _allIcons = null;
        public ObservableCollection<Character> AllIcons
        {
            get => _allIcons;
            set => SetProperty(ref _allIcons, value);
        }

        /// <summary>
        /// 加载本机 Segoe Fluent Icons 字体内的所有图标
        /// </summary>
        public void LoadSegoeFluentIcons()
        {
            if (AllIcons is null)
            {
                AllIcons = new ObservableCollection<Character>();
                var icons = FontHelper.GetAllSegoeFluentIcons();
                foreach (var icon in icons)
                {
                    AllIcons.Add(icon);
                }
            }
        }

        #endregion

        /// <summary>
        /// 根据分类ID获取分类信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CategoryModel GetCategoryById(int id)
        {
            _categoriesDict.TryGetValue(id, out CategoryModel category);
            return category;
        }

        /// <summary>
        /// 重新从数据库加载分类列表
        /// </summary>
        private void LoadCategoriesTable()
        {
            try
            {
                if (PasswordsDataAccess.IsDatabaseConnected())
                {
                    _categoriesDict.Clear();
                    Categoryies.Clear();
                    CategoriesOnNav.Clear();

                    var categories = PasswordsDataAccess.GetCategories();
                    foreach (var item in categories)
                    {
                        var category = new CategoryModel()
                        {
                            Id = item.Id,
                            Title = item.Title,
                            Icon = item.Icon,
                            Order = item.Order,
                        };

                        _categoriesDict[category.Id] = category;
                        Categoryies.Insert(0, category);
                        CategoriesOnNav.Insert(0, new MainNavigationItem(item.Title, $"{HoneypotConsts.CategoryPageTagPrefix}{item.Id}", item.Icon));
                    }
                }
                else
                {
                    InitPasswordsDataBase();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
                ShowTipsContentDialog(resourceLoader.GetString("DialogTitleOops"), $"{resourceLoader.GetString("DialogContentWrongLoadCategories")}: {ex.Message}");
            }
        }

        /// <summary>
        /// 添加分类
        /// </summary>
        /// <param name="title"></param>
        /// <param name="icon"></param>
        public void CreateCategory(string title, string icon)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(title))
                {
                    title = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader().GetString("UnknownCategoryTitle");
                }

                if (string.IsNullOrWhiteSpace(icon))
                {
                    icon = "\uE72E";
                }

                PasswordsDataAccess.AddCategory(title, icon, DateTime.Now.Ticks);

                LoadCategoriesTable();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
                ShowTipsContentDialog(resourceLoader.GetString("DialogTitleOops"), $"{resourceLoader.GetString("DialogContentWrongAddCategories")}: {ex.Message}");
            }
        }

        /// <summary>
        /// 编辑分类信息
        /// </summary>
        /// <param name="category"></param>
        /// <param name="title"></param>
        /// <param name="icon"></param>
        public void EditCategory(CategoryModel category, string title, string icon)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(title))
                {
                    title = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader().GetString("UnknownCategoryTitle");
                }

                if (string.IsNullOrWhiteSpace(icon))
                {
                    icon = "\uE72E";
                }

                PasswordsDataAccess.UpdateCategory(category.Id, title, icon, category.Order);

                LoadPasswordsTable();
                LoadCategoriesTable();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
                ShowTipsContentDialog(resourceLoader.GetString("DialogTitleOops"), $"{resourceLoader.GetString("DialogContentWrongEditCategories")}: {ex.Message}");
            }
        }

        /// <summary>
        /// 将分类移到最前
        /// </summary>
        /// <param name="category"></param>
        public void MoveCategory(CategoryModel category)
        {
            try
            {
                PasswordsDataAccess.UpdateCategory(category.Id, category.Title, category.Icon, DateTime.Now.Ticks);

                LoadCategoriesTable();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
                ShowTipsContentDialog(resourceLoader.GetString("DialogTitleOops"), $"{resourceLoader.GetString("DialogContentWrongMoveCategories")}: {ex.Message}");
            }
        }

        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="id"></param>
        public void DeleteCategory(int id)
        {
            try
            {
                PasswordsDataAccess.DeleteCategory(id);

                LoadPasswordsTable();
                LoadCategoriesTable();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
                ShowTipsContentDialog(resourceLoader.GetString("DialogTitleOops"), $"{resourceLoader.GetString("DialogContentWrongDeleteCategories")}: {ex.Message}");
            }
        }
    }
}
