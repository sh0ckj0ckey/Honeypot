using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using ManyPasswords3.Core;
using ManyPasswords3.Data;
using ManyPasswords3.Models;

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

        public MainViewModel()
        {
            AppSettings.OnAppearanceSettingChanged += (index) => { ActSwitchAppTheme?.Invoke(); };
            AppSettings.OnBackdropSettingChanged += (index) => { ActChangeBackdrop?.Invoke(); };

            // 导航栏
            MainNavigationItems.Add(new MainNavigationItem("所有账号", "all", "\uE8D7"));
            MainNavigationItems.Add(new MainNavigationItem("收藏夹", "favorite", "\uEB51"));
            MainNavigationItems.Add(new MainNavigationItem("全部分类", "category", "\uE74C"/*, MainNavigationRecentClassesItems*/));

            // 导航栏底部
            MainNavigationFooterItems.Add(new MainNavigationItem("密码生成器", "random", "\uF439"));
            MainNavigationFooterItems.Add(new MainNavigationSeparator());
            MainNavigationFooterItems.Add(new MainNavigationSettingItem());

        }

        /// <summary>
        /// 加载数据库
        /// </summary>
        public void LoadPasswordsDataBase()
        {
            //PasswordsDataAccess.InitializeDatabase();
        }
    }
}
