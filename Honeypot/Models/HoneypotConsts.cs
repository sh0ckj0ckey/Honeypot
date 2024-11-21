using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Honeypot.Models
{
    internal static class HoneypotConsts
    {
        public static readonly string AllPasswordsPageTitle = "All Passwords";
        public static readonly string CategoryPageTagPrefix = "category_";

        static HoneypotConsts()
        {
            var resourceLoader = new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
            AllPasswordsPageTitle = resourceLoader.GetString("NavigationItemAllPasswords");
        }
    }
}
