using System.Collections.ObjectModel;

namespace Honeypot.ViewModels;

public class PasswordGroupViewModel
{
    public string Header { get; }

    public string Icon { get; }

    public ObservableCollection<PasswordItemViewModel> Passwords { get; }

    public PasswordGroupViewModel(string header, ObservableCollection<PasswordItemViewModel> passwords)
    {
        this.Header = header;
        this.Icon = string.Empty;
        this.Passwords = passwords;
    }

    public PasswordGroupViewModel(string header, string icon, ObservableCollection<PasswordItemViewModel> passwords)
    {
        this.Header = header;
        this.Icon = icon;
        this.Passwords = passwords;
    }
}
