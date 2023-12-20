using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Honeypot.Data;
using Honeypot.Helpers;
using Honeypot.Models;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

namespace Honeypot.ViewModels
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
            try
            {
                if (PasswordsDataAccess.IsDatabaseConnected())
                {
                    Passwords.Clear();
                    var passwords = PasswordsDataAccess.GetPasswords();
                    foreach (var item in passwords)
                    {
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
                            LogoFileName = item.Logo
                        });
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
            try
            {
                string firstLetter = PinyinHelper.GetFirstSpell(name).ToString();
                string date = DateTime.Now.ToString("yyyy年MM月dd日");
                PasswordsDataAccess.AddPassword(categoryid, account, password, firstLetter, name, date, date, website, note, favorite, logoFilePath);

                LoadPasswordsTable();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                ShowTipsContentDialog("糟糕...", $"添加密码时出现了异常：{ex.Message}");
            }
        }

        /// <summary>
        /// 编辑密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="categoryid"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="name"></param>
        /// <param name="editDate"></param>
        /// <param name="website"></param>
        /// <param name="note"></param>
        /// <param name="favorite"></param>
        /// <param name="image"></param>
        public void EditPassword(int id, int categoryid, string account, string password, string name, string website, string note, bool favorite, string logoFilePath)
        {
            try
            {
                string firstLetter = PinyinHelper.GetFirstSpell(name).ToString();
                string date = DateTime.Now.ToString("yyyy年MM月dd日");
                PasswordsDataAccess.UpdatePassword(id, categoryid, account, password, firstLetter, name, date, website, note, favorite, logoFilePath);

                LoadPasswordsTable();
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
        /// <param name="id"></param>
        public void DeletePassword(PasswordModel password)
        {
            try
            {
                ImageSaveHelper.DeleteLogoImage(password.LogoFileName);
                PasswordsDataAccess.DeletePassword(password.Id);

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
