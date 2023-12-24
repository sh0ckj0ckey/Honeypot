﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Honeypot.Helpers;

namespace Honeypot.Models
{
    public class CategoryModel : ObservableObject
    {
        public int Id { get; set; } = -1;

        public long Order { get; set; } = -1;

        private string _title = "";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _icon = "";
        public string Icon
        {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }
    }
}
