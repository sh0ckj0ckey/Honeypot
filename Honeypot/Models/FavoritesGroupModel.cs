using System.Collections.ObjectModel;

namespace Honeypot.Models
{
    public class FavoritesGroupModel
    {
        public int Key { get; set; }

        public ObservableCollection<PasswordModel> Passwords { get; set; }
    }
}
