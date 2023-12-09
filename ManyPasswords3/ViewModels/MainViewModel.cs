using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using ManyPasswords3.Core;
using ManyPasswords3.Data;
using ManyPasswords3.Helpers;
using ManyPasswords3.Models;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;

namespace ManyPasswords3.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private static Lazy<MainViewModel> _lazyVM = new Lazy<MainViewModel>(() => new MainViewModel());
        public static MainViewModel Instance => _lazyVM.Value;

        public SettingsService AppSettings { get; set; } = new SettingsService();

        /// <summary>
        /// 控制主窗口根据当前的主题进行切换
        /// </summary>
        public Action ActSwitchAppTheme { get; set; } = null;

        /// <summary>
        /// 控制主窗口根据当前的设置更改背景材质
        /// </summary>
        public Action ActChangeBackdrop { get; set; } = null;


        public ObservableCollection<MainNavigationBase> MainNavigationItems = new ObservableCollection<MainNavigationBase>();

        public ObservableCollection<MainNavigationBase> MainNavigationFooterItems = new ObservableCollection<MainNavigationBase>();

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
        /// 所有分类
        /// </summary>
        public ObservableCollection<CategoryModel> Categoryies { get; set; } = new ObservableCollection<CategoryModel>();

        /// <summary>
        /// 当前显示的密码列表
        /// </summary>
        public ObservableCollection<PasswordModel> Passwords { get; set; } = new ObservableCollection<PasswordModel>();

        public MainViewModel()
        {
            AppSettings.OnAppearanceSettingChanged += (index) => { ActSwitchAppTheme?.Invoke(); };
            AppSettings.OnBackdropSettingChanged += (index) => { ActChangeBackdrop?.Invoke(); };

            // 导航栏
            MainNavigationItems.Add(new MainNavigationItem("所有账号", "all", "\uE8D7"));
            MainNavigationItems.Add(new MainNavigationItem("收藏夹", "favorite", "\uEB51"));
            MainNavigationItems.Add(new MainNavigationItem("添加", "adding", "\uE109"));
            MainNavigationItems.Add(new MainNavigationSeparator());
            MainNavigationItems.Add(new MainNavigationItem("全部分类", "category", "\uE74C"/*, MainNavigationRecentClassesItems*/));

            // 导航栏底部
            MainNavigationFooterItems.Add(new MainNavigationItem("密码生成器", "random", "\uF439"));
            MainNavigationFooterItems.Add(new MainNavigationSeparator());
            MainNavigationFooterItems.Add(new MainNavigationSettingItem());

            InitPasswordsDataBase();
        }

        /// <summary>
        /// 加载数据库
        /// </summary>
        public async void InitPasswordsDataBase()
        {
            try
            {
                StorageFolder documentsFolder = await StorageFolder.GetFolderFromPathAsync(UserDataPaths.GetDefault().Documents);
                var noMewingFolder = await documentsFolder.CreateFolderAsync("NoMewing", CreationCollisionOption.OpenIfExists);
                var dbFolder = await noMewingFolder.CreateFolderAsync("ManyPasswords", CreationCollisionOption.OpenIfExists);
                PasswordsDataAccess.CloseDatabase();
                await PasswordsDataAccess.LoadDatabase(dbFolder);

                LoadPasswordsTable();
                LoadCategoriesTable();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 重新从数据库加载所有密码列表
        /// </summary>
        private void LoadPasswordsTable()
        {
            if (PasswordsDataAccess.IsDatabaseConnected())
            {
                Passwords.Clear();
                var passwords = PasswordsDataAccess.GetPasswords();
                foreach (var item in passwords)
                {
                    try
                    {
                        BitmapImage avatarImage = null;

                        Passwords.Insert(0, new PasswordModel()
                        {
                            Id = item.Id,
                            Account = item.Account,
                            Password = item.Password,
                            FirstLetter = item.FirstLetter[0],
                            Name = item.Name,
                            CreateDate = item.CreateDate,
                            EditDate = item.EditDate,
                            Website = item.Website,
                            Note = item.Note,
                            Favorite = item.Favorite != 0,
                            Image = avatarImage
                        });
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }
            }
            else
            {
                InitPasswordsDataBase();
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
                var categories = PasswordsDataAccess.GetCategories();
                foreach (var item in categories)
                {
                    Categoryies.Insert(0, new CategoryModel()
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Icon = item.Icon,
                    });
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
                icon = "\uE003";
            }

            PasswordsDataAccess.AddOneCategory(title, icon);

            LoadCategoriesTable();
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
    }
}
