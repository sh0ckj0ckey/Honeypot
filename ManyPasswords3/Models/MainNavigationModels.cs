﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace ManyPasswords3.Models
{
    public class MainNavigationBase { }

    public class MainNavigationItem : MainNavigationBase
    {
        public string sName { get; set; }
        public string sTag { get; set; }
        public string sIcon { get; set; }
        public bool bIsLeaf { get; set; }
        public ObservableCollection<MainNavigationItem> vChildren { get; set; } = null;

        public MainNavigationItem(string name, string tag, string icon, ObservableCollection<MainNavigationItem> children = null)
        {
            sName = name;
            sTag = tag;
            sIcon = icon;
            bIsLeaf = children == null;
            vChildren = children;
        }
    }

    public class MainNavigationHeader : MainNavigationBase
    {
        public string sName { get; set; }

        public MainNavigationHeader(string name)
        {
            sName = name;
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
