using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Honeypot.Models;

public abstract class NavigationBase
{

}

public sealed class NavigationItem(string name, string tag, string icon, ObservableCollection<NavigationItem>? children = null) : NavigationBase
{
    public string Name { get; } = name;

    public string Tag { get; } = tag;

    public string Icon { get; } = icon;

    public ObservableCollection<NavigationItem> Children { get; } = children ?? [];
}

public sealed class NavigationHeader(string name) : NavigationBase
{
    public string Name { get; } = name;
}

public sealed class NavigationSeparator : NavigationBase
{

}

public sealed class NavigationSettingsItem(string name) : NavigationBase
{
    public string Name { get; } = name;
}

public partial class NavigationItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate? ItemTemplate { get; set; }

    public DataTemplate? HeaderTemplate { get; set; }

    public DataTemplate? SeparatorTemplate { get; set; }

    public DataTemplate? SettingsItemTemplate { get; set; }

    protected override DataTemplate? SelectTemplateCore(object item)
    {
        return item switch
        {
            NavigationItem _ => ItemTemplate,
            NavigationHeader _ => HeaderTemplate,
            NavigationSeparator _ => SeparatorTemplate,
            NavigationSettingsItem _ => SettingsItemTemplate,
            _ => base.SelectTemplateCore(item)
        };
    }
}
