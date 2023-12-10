﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using ManyPasswords3.Helpers;

namespace ManyPasswords3.Models
{
    public class MainNavigationBase : ObservableObject { }

    public class MainNavigationItem : MainNavigationBase
    {
        public string Name { get; set; }
        public string Tag { get; set; }
        public string Icon { get; set; }
        public bool IsLeaf { get; set; }

        private ObservableCollection<MainNavigationItem> _children = null;
        public ObservableCollection<MainNavigationItem> Children
        {
            get => _children;
            set => SetProperty(ref _children, value);
        }

        public MainNavigationItem(string name, string tag, string icon, ObservableCollection<MainNavigationItem> children = null)
        {
            Name = name;
            Tag = tag;
            Icon = icon;
            IsLeaf = children == null;
            Children = children;
        }
    }

    public class MainNavigationHeader : MainNavigationBase
    {
        public string Name { get; set; }

        public MainNavigationHeader(string name)
        {
            Name = name;
        }
    }

    public class MainNavigationSeparator : MainNavigationBase
    {

    }

    public class MainNavigationSettingItem : MainNavigationBase
    {

    }

    class MenuItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ItemTemplate { get; set; }
        public DataTemplate HeaderTemplate { get; set; }
        public DataTemplate SeparatorTemplate { get; set; }
        public DataTemplate SettingItemTemplate { get; set; }
        protected override DataTemplate SelectTemplateCore(object item)
        {
            return item is MainNavigationItem ? ItemTemplate : item is MainNavigationHeader ? HeaderTemplate : item is MainNavigationSettingItem ? SettingItemTemplate : SeparatorTemplate;
        }
    }

}
