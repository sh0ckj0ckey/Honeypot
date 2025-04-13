using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Honeypot.Models;
using Honeypot.ViewModels;

namespace Honeypot.Helpers
{
    public static class PasswordsGetter
    {
        public static PasswordModel GetPasswordById(int id)
        {
            return MainViewModel.Instance.GetPasswordById(id);
        }

        public static CategoryModel GetCategoryById(int id)
        {
            return MainViewModel.Instance.GetCategoryById(id);
        }
    }
}
