using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Honeypot.Models
{
    public class LegacyPasswordItem
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string sAccount { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string sPassword { get; set; }

        /// <summary>
        /// 名字的首字母
        /// </summary>
        public char sFirstLetter { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        public string sName { get; set; }

        /// <summary>
        /// 添加日期
        /// </summary>
        public string sDate { get; set; }

        /// <summary>
        /// 最近一次修改日期
        /// </summary>
        public string sEditDate { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string sPicture { get; set; }

        /// <summary>
        /// 网址
        /// </summary>
        public string sWebsite { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string sNote { get; set; }

        /// <summary>
        /// 收藏
        /// </summary>
        public bool bFavorite { get; set; }
    }
}
