using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyPasswords.Models
{
    /// <summary>
    /// 废弃，仅作兼容旧版本用
    /// </summary>
    public class OnePassword
    {
        /// <summary>
        /// 名字的首字母
        /// </summary>
        public char FirstLetter { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 图片名（英文）
        /// </summary>
        public string ImageName { get; set; }

        /// <summary>
        /// 添加日期
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Picture { get; set; }

        /// <summary>
        /// 网址
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public double Priority { get; set; }

        /// <summary>
        /// 收藏
        /// </summary>
        public bool IsFavorite { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }


        /// <summary>
        /// 默认构造函数,初始化一些属性
        /// </summary>
        public OnePassword()
        {
            this.Name = "未命名";
            this.Info = "没有写过备注哦";
            this.Picture = "ms-appx:///Assets/BuildInIcon/default.jpg";
            this.Website = "";
            this.Account = "未添加";
            this.Password = "";
            this.IsFavorite = false;
            this.Priority = 0;
            this.Date = "创建于 " + DateTime.Now.ToLongDateString().ToString();
            this.FirstLetter = 'W';
        }

        //*************************************************下面废弃******************************************

        /// <summary>
        /// 完整信息后的账户,所有属性都将有值
        /// </summary>
        /// <param name="imagePath">完整的图片路径</param>
        /// <param name="name"></param>
        /// <param name="info"></param>
        /// <param name="website"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="priority"></param>
        public OnePassword(string imagePath, string name, string info, string website, string account, string password, double priority, char firstLetter)
        {
            this.Name = name;
            this.Info = info;
            this.ImageName = "Customize";
            this.Picture = imagePath;
            this.Website = website;
            this.Account = account;
            this.Password = password;
            this.IsFavorite = false;
            this.Priority = priority;
            this.Date = "创建于 " + DateTime.Now.ToLongDateString().ToString();
            this.FirstLetter = firstLetter;
        }

        /// <summary>
        /// 使用内置账户模板时传递这个对象
        /// </summary>
        /// <param name="imageName"></param>
        /// <param name="name"></param>
        /// <param name="info"></param>
        /// <param name="website"></param>
        public OnePassword(string imageName, string name, string info, string website, char firstLetter)
        {
            this.ImageName = imageName;
            this.Name = name;
            this.Info = info;
            this.Account = "";
            this.Password = "";
            this.IsFavorite = false;
            this.Picture = "ms-appx:///Assets/BuildInIcon/" + imageName + ".jpg";
            this.Website = website;
            this.IsFavorite = false;
            this.Priority = 0;
            this.Date = "创建于 " + DateTime.Now.ToLongDateString().ToString();
            this.FirstLetter = firstLetter;
        }
    }

    /// <summary>
    /// 废弃，仅作兼容旧版本用
    /// </summary>
    public class PasswordsInGroup
    {
        public char Key { get; set; }
        public List<OnePassword> PasswordsContent { get; set; }
    }
}
