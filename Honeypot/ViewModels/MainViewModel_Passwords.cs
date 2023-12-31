using System;
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
        /// 当前显示的密码列表
        /// </summary>
        public ObservableCollection<PasswordModel> Passwords { get; set; } = new();

        /// <summary>
        /// 当前显示的分组的密码列表
        /// </summary>
        public ObservableCollection<PasswordsGroupModel> PasswordsGroups { get; set; } = new();

        /// <summary>
        /// 收藏夹分组列表
        /// </summary>
        public ObservableCollection<FavoritesGroupModel> FavoritePasswordsGroups { get; set; } = new();

        /// <summary>
        /// 搜索建议列表
        /// </summary>
        public ObservableCollection<PasswordModel> SearchSuggestPasswords { get; set; } = new();

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
        /// 当前选中查看的收藏的密码
        /// </summary>
        private PasswordModel _selectedFavoritePassword = null;
        public PasswordModel SelectedFavoritePassword
        {
            get => _selectedFavoritePassword;
            set => SetProperty(ref _selectedFavoritePassword, value);
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
                    Passwords.Clear();
                    PasswordsGroups.Clear();
                    SearchSuggestPasswords.Clear();

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
                            Logo = await LogoImageHelper.GetLogoImage(item.Logo)
                        };

                        Passwords.Insert(0, password);
                    }

                    // 按照首字母分组
                    var orderedList =
                        (from item in Passwords
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

                    UpdateFavorites();
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
        /// 更新收藏夹列表
        /// </summary>
        private void UpdateFavorites()
        {
            FavoritePasswordsGroups.Clear();

            // 收藏夹
            var orderedFavoriteList =
                (from item in Passwords
                 where item.Favorite
                 group item by item.CategoryId into newItems
                 select
                 new FavoritesGroupModel
                 {
                     Key = newItems.Key,
                     Passwords = new ObservableCollection<PasswordModel>(newItems.ToList())
                 }).OrderBy(x => x.Key).ToList();

            foreach (var item in orderedFavoriteList)
            {
                FavoritePasswordsGroups.Add(item);
            }
        }

        /// <summary>
        /// 添加密码
        /// </summary>
        /// <param name="categoryid"></param>
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
        public void AddPassword(int categoryid, string account, string password, string name, string website, string note, bool favorite, string logoFilePath)
        {
            string firstLetter = PinyinHelper.GetFirstSpell(name).ToString();
            string date = DateTime.Now.ToString("yyyy年MM月dd日");
            PasswordsDataAccess.AddPassword(categoryid, account, password, firstLetter, name, date, website, note, favorite, logoFilePath);

            LoadPasswordsTable();
        }

        /// <summary>
        /// 编辑密码
        /// </summary>
        /// <param name="password"></param>
        /// <param name="categoryid"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="name"></param>
        /// <param name="editDate"></param>
        /// <param name="website"></param>
        /// <param name="note"></param>
        /// <param name="favorite"></param>
        /// <param name="image"></param>
        public void EditPassword(PasswordModel passwordItem, int categoryid, string account, string password, string name, string website, string note, bool favorite, string logoFilePath)
        {
            try
            {
                if (logoFilePath != passwordItem.LogoFileName)
                {
                    LogoImageHelper.DeleteLogoImage(passwordItem.LogoFileName);
                }

                string firstLetter = PinyinHelper.GetFirstSpell(name).ToString();
                string date = DateTime.Now.ToString("yyyy年MM月dd日");
                PasswordsDataAccess.UpdatePassword(passwordItem.Id, categoryid, account, password, firstLetter, name, date, website, note, favorite, logoFilePath);

                LoadPasswordsTable();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                ShowTipsContentDialog("糟糕...", $"编辑密码时出现了异常：{ex.Message}");
            }
        }

        /// <summary>
        /// 收藏/取消收藏密码
        /// </summary>
        /// <param name="passwordItem"></param>
        public void FavoritePassword(PasswordModel passwordItem)
        {
            try
            {
                passwordItem.Favorite = !passwordItem.Favorite;
                PasswordsDataAccess.FavoritePassword(passwordItem.Id, passwordItem.Favorite);

                UpdateFavorites();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                ShowTipsContentDialog("糟糕...", $"收藏密码时出现了异常：{ex.Message}");
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
