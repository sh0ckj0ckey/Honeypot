using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Honeypot.Models;

namespace Honeypot.ViewModels
{
    public partial class MainViewModel
    {
        /// <summary>
        /// 在当前分类下搜索密码
        /// </summary>
        /// <param name="search"></param>
        public List<PasswordModel> SearchPasswords(string search)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var suggests = Passwords.Where(p => (p.Name.StartsWith(search, StringComparison.CurrentCultureIgnoreCase)
                                                                                        || p.Account.StartsWith(search, StringComparison.CurrentCultureIgnoreCase)
                                                                                        /*|| p.Website.Contains(search, StringComparison.CurrentCultureIgnoreCase)*/)).ToList();
                    return suggests;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return null;
        }
    }
}
