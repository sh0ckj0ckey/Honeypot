using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Honeypot.Data;
using Honeypot.Helpers;
using Honeypot.Models;

namespace Honeypot.ViewModels
{
    public partial class MainViewModel
    {
        /// <summary>
        /// 所有密码ID与密码信息的映射
        /// </summary>
        private Dictionary<int, PasswordModel> _passwordsDict = new Dictionary<int, PasswordModel>();

        /// <summary>
        /// 所有密码列表
        /// </summary>
        public ObservableCollection<PasswordModel> AllPasswords { get; set; } = new();

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
        /// 根据密码ID获取密码信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PasswordModel GetPasswordById(int id)
        {
            _passwordsDict.TryGetValue(id, out PasswordModel password);
            return password;
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
                    _passwordsDict.Clear();
                    AllPasswords.Clear();
                    SelectedPassword = null;

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
                            ThirdPartyId = item.ThirdPartyId,
                            LogoFileName = item.Logo,
                        };

                        _passwordsDict[password.Id] = password;
                        AllPasswords.Insert(0, password);
                    }

                    ActNoticeUserToBackup?.Invoke(AllPasswords.Count);

                    UpdatePasswordsList(PasswordsCategoryId);
                    UpdateFavorites();

                    foreach (var password in AllPasswords)
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

                var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
                ShowTipsContentDialog(resourceLoader.GetString("DialogTitleOops"), $"{resourceLoader.GetString("DialogContentWrongLoadPasswords")}: {ex.Message}");
            }
        }

        /// <summary>
        /// 更新密码列表
        /// </summary>
        /// <param name="categoryId"></param>
        public void UpdatePasswordsList(int categoryId, int keepSelectId = -1)
        {
            Debug.WriteLine($"Updateing Passwords, CategoryId = {categoryId}");

            PasswordsCategoryId = categoryId;
            PasswordsTitleText = categoryId > 0 ? GetCategoryById(categoryId).Title : HoneypotConsts.AllPasswordsPageTitle;
            PasswordsTitleIcon = categoryId > 0 ? GetCategoryById(categoryId).Icon : "";

            Passwords.Clear();
            PasswordsGroups.Clear();

            PasswordModel selectedPassword = null;

            if (PasswordsCategoryId <= 0)
            {
                // 按照添加顺序排列
                foreach (var item in AllPasswords)
                {
                    Passwords.Add(item);

                    if (keepSelectId > 0 && item.Id == keepSelectId)
                    {
                        selectedPassword = item;
                    }
                }

                // 按照首字母分组
                var orderedList =
                    (from item in AllPasswords
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
                foreach (var item in AllPasswords)
                {
                    if (item.CategoryId == PasswordsCategoryId)
                    {
                        Passwords.Add(item);

                        if (keepSelectId > 0 && item.Id == keepSelectId)
                        {
                            selectedPassword = item;
                        }
                    }
                }

                // 按照首字母分组
                var orderedList =
                    (from item in AllPasswords
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

            SelectedPassword = selectedPassword;
        }

        /// <summary>
        /// 添加密码
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="name"></param>
        /// <param name="website"></param>
        /// <param name="note"></param>
        /// <param name="favorite"></param>
        /// <param name="logoFilePath"></param>
        /// <param name="date"></param>
        public void AddPassword(int categoryId, string account, string password, int thirdPartyLoginId, string name, string website, string note, bool favorite, string logoFilePath, string date = "")
        {
            string firstLetter = PinyinHelper.GetFirstSpell(name.Trim()).ToString();
            if (string.IsNullOrWhiteSpace(date))
            {
                date = DateTime.Now.ToString("yyyy/MM/dd");
            }

            PasswordsDataAccess.AddPassword(categoryId, account, password, thirdPartyLoginId, firstLetter, name, date, website, note, favorite, logoFilePath);
            LoadPasswordsTable();
        }

        /// <summary>
        /// 编辑密码
        /// </summary>
        /// <param name="passwordItem"></param>
        /// <param name="categoryId"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="name"></param>
        /// <param name="website"></param>
        /// <param name="note"></param>
        /// <param name="favorite"></param>
        /// <param name="logoFilePath"></param>
        public async void EditPassword(PasswordModel passwordItem, int categoryId, string account, string password, int thirdPartyLoginId, string name, string website, string note, bool favorite, string logoFilePath)
        {
            try
            {
                if (logoFilePath != passwordItem.LogoFileName)
                {
                    LogoImageHelper.DeleteLogoImage(passwordItem.LogoFileName);
                }

                string firstLetter = PinyinHelper.GetFirstSpell(name).ToString();
                string date = DateTime.Now.ToString("yyyy/MM/dd");

                PasswordsDataAccess.UpdatePassword(passwordItem.Id, categoryId, account, password, thirdPartyLoginId, firstLetter, name, date, website, note, favorite, logoFilePath);

                passwordItem.Account = account;
                passwordItem.Password = password;
                passwordItem.ThirdPartyId = thirdPartyLoginId;
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

                UpdatePasswordsList(PasswordsCategoryId, passwordItem.Id);
                UpdateFavorites();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
                ShowTipsContentDialog(resourceLoader.GetString("DialogTitleOops"), $"{resourceLoader.GetString("DialogContentWrongEditPasswords")}: {ex.Message}");
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

                var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
                ShowTipsContentDialog(resourceLoader.GetString("DialogTitleOops"), $"{resourceLoader.GetString("DialogContentWrongDeletePasswords")}: {ex.Message}");
            }
        }
    }
}
