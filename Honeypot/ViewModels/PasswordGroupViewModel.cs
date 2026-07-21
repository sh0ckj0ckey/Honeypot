using System.Collections.ObjectModel;

namespace Honeypot.ViewModels;

public class PasswordGroupViewModel(string key, ObservableCollection<PasswordItemViewModel> passwords)
{
    public string Key { get; } = key;

    public ObservableCollection<PasswordItemViewModel> Passwords { get; } = passwords;
}
