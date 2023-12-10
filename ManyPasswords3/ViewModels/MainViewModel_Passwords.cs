using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManyPasswords3.Data;
using ManyPasswords3.Models;
using Microsoft.UI.Xaml.Media.Imaging;

namespace ManyPasswords3.ViewModels
{
    public partial class MainViewModel
    {
        /// <summary>
        /// 当前显示的密码列表
        /// </summary>
        public ObservableCollection<PasswordModel> Passwords { get; set; } = new ObservableCollection<PasswordModel>();

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
    }
}
