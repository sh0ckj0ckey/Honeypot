using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Honeypot.Data;
using Honeypot.Helpers;
using Honeypot.Models;
using WinUIEx;

namespace Honeypot.ViewModels
{
    public partial class MainViewModel
    {
        /// <summary>
        /// 所有分类
        /// </summary>
        public ObservableCollection<CategoryModel> Categoryies { get; set; } = new ObservableCollection<CategoryModel>();

        /// <summary>
        /// 显示在侧边导航栏的分类列表，与Categories同步变更
        /// </summary>
        public ObservableCollection<MainNavigationItem> CategoriesOnNav { get; set; } = new ObservableCollection<MainNavigationItem>();

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

        /// <summary>
        /// 重新从数据库加载分类列表
        /// </summary>
        private void LoadCategoriesTable()
        {
            if (PasswordsDataAccess.IsDatabaseConnected())
            {
                Categoryies.Clear();
                CategoriesOnNav.Clear();
                var categories = PasswordsDataAccess.GetCategories();
                foreach (var item in categories)
                {
                    Categoryies.Insert(0, new CategoryModel()
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Icon = item.Icon,
                    });

                    CategoriesOnNav.Insert(0, new MainNavigationItem(item.Title, $"category_{item.Id}", item.Icon));
                }
            }
            else
            {
                InitPasswordsDataBase();
            }
        }

        /// <summary>
        /// 添加分类
        /// </summary>
        /// <param name="title"></param>
        /// <param name="icon"></param>
        public void CreateCategory(string title, string icon)
        {
            if (string.IsNullOrEmpty(title))
            {
                title = "未命名分类";
            }

            if (string.IsNullOrEmpty(icon))
            {
                icon = "\uE72E";
            }

            PasswordsDataAccess.AddOneCategory(title, icon);

            LoadCategoriesTable();
        }

        /// <summary>
        /// 编辑分类信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="icon"></param>
        public void EditCategory(int id, string title, string icon)
        {
            if (string.IsNullOrEmpty(title))
            {
                title = "未命名分类";
            }

            if (string.IsNullOrEmpty(icon))
            {
                icon = "\uE003";
            }

            PasswordsDataAccess.UpdateOneCategory(id, title, icon);

            LoadCategoriesTable();
        }

        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="id"></param>
        public void DeleteCategory(int id)
        {
            PasswordsDataAccess.DeleteOneCategory(id);

            LoadCategoriesTable();
        }
    }
}
