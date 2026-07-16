using System.Collections.Generic;
using System.Collections.ObjectModel;
using Honeypot.Helpers;
using Honeypot.Models;

namespace Honeypot.ViewModels;

public class NavigationViewModel
{
    private readonly ObservableCollection<NavigationItem> _categoryNavigationItems = [];

    public ObservableCollection<NavigationBase> Items { get; } = [];

    public ObservableCollection<NavigationBase> FooterItems { get; } = [];

    public NavigationViewModel()
    {
        this.Items.Add(new NavigationItem("NavigationItemAllPasswords".GetLocalized(), "passwords", "\uE8D7"));
        this.Items.Add(new NavigationItem("NavigationItemFavorites".GetLocalized(), "favorites", "\uEB51"));
        this.Items.Add(new NavigationItem("NavigationItemAdd".GetLocalized(), "adding", "\uE109"));
        this.Items.Add(new NavigationSeparator());
        this.Items.Add(new NavigationItem("NavigationItemCategories".GetLocalized(), "categories", "\uE74C", _categoryNavigationItems));

        this.FooterItems.Add(new NavigationItem("NavigationItemGenerator".GetLocalized(), "random", "\uF439"));
        this.FooterItems.Add(new NavigationSeparator());
        this.FooterItems.Add(new NavigationItem("NavigationItemTips".GetLocalized(), "tips", "\uE82F"));
        this.FooterItems.Add(new NavigationSettingsItem("NavigationItemSettings".GetLocalized(), "settings"));
    }

    public void UpdateCategories(IEnumerable<CategoryItemViewModel> categories)
    {
        _categoryNavigationItems.Clear();

        foreach (var category in categories)
        {
            _categoryNavigationItems.Add(new NavigationItem(category.Title, $"category:{category.Id}", category.Icon));
        }
    }
}
