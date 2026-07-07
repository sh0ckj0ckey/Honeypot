using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Honeypot.Data.Models;

namespace Honeypot.ViewModels;

/// <summary>
/// Represents a password category item displayed in the UI.
/// </summary>
public partial class CategoryItemViewModel : ObservableObject
{
    private int _id = -1;

    private string _title = string.Empty;

    private string _icon = string.Empty;

    private long _order;

    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string Icon
    {
        get => _icon;
        set => SetProperty(ref _icon, value);
    }

    public long Order
    {
        get => _order;
        set => SetProperty(ref _order, value);
    }

    public CategoryItemViewModel(CategoryModel categoryModel)
    {
        ArgumentNullException.ThrowIfNull(categoryModel);

        _id = categoryModel.Id;
        _title = categoryModel.Title;
        _icon = categoryModel.Icon;
        _order = categoryModel.Order;
    }
}