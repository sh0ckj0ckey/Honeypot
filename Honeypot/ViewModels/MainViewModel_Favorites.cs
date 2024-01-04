using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Honeypot.Data;
using Honeypot.Helpers;
using Honeypot.Models;

namespace Honeypot.ViewModels
{
    public partial class MainViewModel
    {
        /// <summary>
        /// 收藏夹分组列表
        /// </summary>
        public ObservableCollection<FavoritesGroupModel> FavoritePasswordsGroups { get; set; } = new();

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
        /// 更新收藏夹列表
        /// </summary>
        private void UpdateFavorites()
        {
            FavoritePasswordsGroups.Clear();

            // 收藏夹
            var orderedFavoriteList =
                (from item in _allPasswords
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
        /// 收藏/取消收藏密码
        /// </summary>
        /// <param name="passwordItem"></param>
        public async void FavoritePassword(PasswordModel passwordItem)
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
    }
}
