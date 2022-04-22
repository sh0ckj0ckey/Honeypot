using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyPasswords.Models
{
    internal class PasswordsGroup
    {
        public char Key { get; set; }
        public ObservableCollection<OnePassword> PasswordsContent { get; set; }
    }
}
