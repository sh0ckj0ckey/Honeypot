using System.Collections.ObjectModel;

namespace Honeypot.Models
{
    public class PasswordsGroupModel
    {
        public char Key { get; set; }

        public ObservableCollection<PasswordModel> Passwords { get; set; }
    }
}
