using System.Collections.ObjectModel;

namespace Honeypot.ViewModels;

public class PasswordGroupViewModel(string key)
{
    public string Key { get; } = key;

    public ObservableCollection<PasswordItemViewModel> Passwords { get; } = [];
}
