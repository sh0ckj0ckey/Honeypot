using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Honeypot.Data;
using Honeypot.Helpers;
using Honeypot.Models;
using Windows.ApplicationModel.Search.Core;

namespace Honeypot.ViewModels
{
    public partial class MainViewModel
    {
        /// <summary>
        /// 所有密码列表
        /// </summary>
        private List<PasswordModel> _allPasswords { get; set; } = new List<PasswordModel>();

        /// <summary>
        /// 当前显示的密码列表（根据分类ID过滤）
        /// </summary>
        public ObservableCollection<PasswordModel> Passwords { get; set; } = new();

        /// <summary>
        /// 当前显示的分组的密码列表（根据分类ID过滤）
        /// </summary>
        public ObservableCollection<PasswordsGroupModel> PasswordsGroups { get; set; } = new();

        /// <summary>
        /// 当前的分类ID， -1为全部密码
        /// 这个属性一方面用于ViewModel去筛选特定分类的密码列表，另一方面也用于定位侧边导航栏的选中项
        /// </summary>
        private int _passwordsCategoryId = -1;
        public int PasswordsCategoryId
        {
            get => _passwordsCategoryId;
            private set => SetProperty(ref _passwordsCategoryId, value);
        }

        /// <summary>
        /// 当前的分类名称， 默认为“所有账号”
        /// 这个属性一方面用于ViewModel去筛选特定分类的密码列表，另一方面也用于定位侧边导航栏的选中项
        /// </summary>
        private string _passwordsTitleText = HoneypotConsts.AllPasswordsPageTitle;
        public string PasswordsTitleText
        {
            get => _passwordsTitleText;
            private set => SetProperty(ref _passwordsTitleText, value);
        }

        /// <summary>
        /// 当前的分类图标，默认为空
        /// 这个属性一方面用于ViewModel去筛选特定分类的密码列表，另一方面也用于定位侧边导航栏的选中项
        /// </summary>
        private string _passwordsTitleIcon = string.Empty;
        public string PasswordsTitleIcon
        {
            get => _passwordsTitleIcon;
            private set => SetProperty(ref _passwordsTitleIcon, value);
        }

        /// <summary>
        /// 当前选中查看的密码
        /// </summary>
        private PasswordModel _selectedPassword = null;
        public PasswordModel SelectedPassword
        {
            get => _selectedPassword;
            set => SetProperty(ref _selectedPassword, value);
        }

        /// <summary>
        /// 重新从数据库加载所有密码列表
        /// </summary>
        private async void LoadPasswordsTable()
        {
            try
            {
                if (PasswordsDataAccess.IsDatabaseConnected())
                {
                    SelectedPassword = null;
                    _allPasswords.Clear();

                    var passwords = PasswordsDataAccess.GetPasswords();
                    foreach (var item in passwords)
                    {
                        var password = new PasswordModel
                        {
                            Id = item.Id,
                            Account = item.Account,
                            Password = item.Password,
                            FirstLetter = item.FirstLetter,
                            Name = item.Name,
                            CreateDate = item.CreateDate,
                            EditDate = item.EditDate,
                            Website = item.Website,
                            Note = item.Note,
                            Favorite = item.Favorite != 0,
                            CategoryId = item.CategoryId,
                            LogoFileName = item.Logo,
                        };

                        _allPasswords.Insert(0, password);
                    }

                    ActNoticeUserToBackup?.Invoke(_allPasswords.Count);

                    UpdatePasswords(PasswordsCategoryId);
                    UpdateFavorites();

                    foreach (var password in _allPasswords)
                    {
                        password.NormalLogoImage = await LogoImageHelper.GetLogoImage(password.LogoFileName, LogoSizeEnum.Medium);
                        password.LargeLogoImage = await LogoImageHelper.GetLogoImage(password.LogoFileName, LogoSizeEnum.Large);
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
                ShowTipsContentDialog("糟糕...", $"读取密码列表时出现了异常：{ex.Message}");
            }
        }

        /// <summary>
        /// 更新密码列表
        /// </summary>
        /// <param name="categoryId"></param>
        public void UpdatePasswords(int categoryId)
        {
            Debug.WriteLine($"Updateing Passwords, CategoryId = {categoryId}");

            PasswordsCategoryId = categoryId;
            PasswordsTitleText = categoryId > 0 ? GetCategoryById(categoryId).Title : HoneypotConsts.AllPasswordsPageTitle;
            PasswordsTitleIcon = categoryId > 0 ? GetCategoryById(categoryId).Icon : "";

            Passwords.Clear();
            PasswordsGroups.Clear();

            if (PasswordsCategoryId <= 0)
            {
                // 按照添加顺序排列
                foreach (var item in _allPasswords)
                {
                    Passwords.Add(item);
                }

                // 按照首字母分组
                var orderedList =
                    (from item in _allPasswords
                     group item by item.FirstLetter into newItems
                     select
                     new PasswordsGroupModel
                     {
                         Key = newItems.Key,
                         Passwords = new ObservableCollection<PasswordModel>(newItems.ToList())
                     }).OrderBy(x => x.Key).ToList();

                foreach (var item in orderedList)
                {
                    PasswordsGroups.Add(item);
                }
            }
            else
            {
                // 按照添加顺序排列
                foreach (var item in _allPasswords)
                {
                    if (item.CategoryId == PasswordsCategoryId)
                    {
                        Passwords.Add(item);
                    }
                }

                // 按照首字母分组
                var orderedList =
                    (from item in _allPasswords
                     where item.CategoryId == PasswordsCategoryId
                     group item by item.FirstLetter into newItems
                     select
                     new PasswordsGroupModel
                     {
                         Key = newItems.Key,
                         Passwords = new ObservableCollection<PasswordModel>(newItems.ToList())
                     }).OrderBy(x => x.Key).ToList();

                foreach (var item in orderedList)
                {
                    PasswordsGroups.Add(item);
                }
            }
        }

        /// <summary>
        /// 添加密码
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="firstLetter"></param>
        /// <param name="name"></param>
        /// <param name="createDate"></param>
        /// <param name="editDate"></param>
        /// <param name="website"></param>
        /// <param name="note"></param>
        /// <param name="favorite"></param>
        /// <param name="image"></param>
        public void AddPassword(int categoryId, string account, string password, string name, string website, string note, bool favorite, string logoFilePath)
        {
            string firstLetter = PinyinHelper.GetFirstSpell(name.Trim()).ToString();
            string date = DateTime.Now.ToString("yyyy年MM月dd日");
            PasswordsDataAccess.AddPassword(categoryId, account, password, firstLetter, name, date, website, note, favorite, logoFilePath);

            LoadPasswordsTable();
        }

        /// <summary>
        /// 编辑密码
        /// </summary>
        /// <param name="password"></param>
        /// <param name="categoryId"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="name"></param>
        /// <param name="editDate"></param>
        /// <param name="website"></param>
        /// <param name="note"></param>
        /// <param name="favorite"></param>
        /// <param name="image"></param>
        public async void EditPassword(PasswordModel passwordItem, int categoryId, string account, string password, string name, string website, string note, bool favorite, string logoFilePath)
        {
            try
            {
                if (logoFilePath != passwordItem.LogoFileName)
                {
                    LogoImageHelper.DeleteLogoImage(passwordItem.LogoFileName);
                }

                string firstLetter = PinyinHelper.GetFirstSpell(name).ToString();
                string date = DateTime.Now.ToString("yyyy年MM月dd日");
                PasswordsDataAccess.UpdatePassword(passwordItem.Id, categoryId, account, password, firstLetter, name, date, website, note, favorite, logoFilePath);

                // 编辑密码后，不需要重新加载数据库，只需要更新对应的属性并刷新列表
                // LoadPasswordsTable();

                passwordItem.Account = account;
                passwordItem.Password = password;
                passwordItem.FirstLetter = firstLetter[0];
                passwordItem.Name = name;
                passwordItem.EditDate = date;
                passwordItem.Website = website;
                passwordItem.Note = note;
                passwordItem.Favorite = favorite;
                passwordItem.CategoryId = categoryId;
                passwordItem.LogoFileName = logoFilePath;

                passwordItem.NormalLogoImage = await LogoImageHelper.GetLogoImage(logoFilePath, LogoSizeEnum.Medium);
                passwordItem.LargeLogoImage = await LogoImageHelper.GetLogoImage(logoFilePath, LogoSizeEnum.Large);

                UpdatePasswords(PasswordsCategoryId);
                UpdateFavorites();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                ShowTipsContentDialog("糟糕...", $"编辑密码时出现了异常：{ex.Message}");
            }
        }

        /// <summary>
        /// 删除密码
        /// </summary>
        /// <param name="passwordItem"></param>
        public void DeletePassword(PasswordModel passwordItem)
        {
            try
            {
                LogoImageHelper.DeleteLogoImage(passwordItem.LogoFileName);
                PasswordsDataAccess.DeletePassword(passwordItem.Id);

                LoadPasswordsTable();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                ShowTipsContentDialog("糟糕...", $"删除密码时出现了异常：{ex.Message}");
            }
        }
    }
}
