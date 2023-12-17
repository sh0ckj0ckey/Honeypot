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
            if (PasswordsDataAccess.IsDatabaseConnected())
            {
                Passwords.Clear();
                var passwords = PasswordsDataAccess.GetPasswords();
                foreach (var item in passwords)
                {
                    try
                    {
                        WriteableBitmap avatarImage = new WriteableBitmap(1, 1);
                        //Stream stream = avatarImage.PixelBuffer.AsStream();
                        //stream.Seek(0, SeekOrigin.Begin);
                        //stream.Write(item.Image, 0, 4 * 192 * 192);

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
        public void AddPassword(int categoryid, string account, string password, string name, string website, string note, bool favorite, byte[] image)
        {
            string firstLetter = PinyinHelper.GetFirstSpell(name).ToString();
            string date = DateTime.Now.ToString("D");
            PasswordsDataAccess.AddPassword(categoryid, account, password, firstLetter, name, date, date, website, note, favorite, image);

            LoadPasswordsTable();
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
        public void EditPassword(int id, int categoryid, string account, string password, string name, string website, string note, bool favorite, byte[] image)
        {
            string firstLetter = PinyinHelper.GetFirstSpell(name).ToString();
            string date = DateTime.Now.ToString("D");
            PasswordsDataAccess.UpdatePassword(id, categoryid, account, password, firstLetter, name, date, website, note, favorite, image);

            LoadPasswordsTable();
        }

        /// <summary>
        /// 删除密码
        /// </summary>
        /// <param name="id"></param>
        public void DeletePassword(int id)
        {
            PasswordsDataAccess.DeletePassword(id);

            LoadPasswordsTable();
        }
    }
}
