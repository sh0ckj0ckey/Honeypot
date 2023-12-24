using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Honeypot.Models
{
    public class PasswordsGroupModel
    {
        public char Key { get; set; }

        public ObservableCollection<PasswordModel> Passwords { get; set; }
    }
}
