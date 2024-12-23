﻿using System;
using Honeypot.ViewModels;
using Microsoft.UI.Xaml.Data;

namespace Honeypot.Converters
{
    class CategoryId2CategoryConverter : IValueConverter
    {
        private static Microsoft.Windows.ApplicationModel.Resources.ResourceLoader _resourceLoader = null;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                string id = value?.ToString();
                if (int.TryParse(id, out int categoryId))
                {
                    string para = parameter?.ToString();
                    if (para == "Icon")
                    {
                        string icon = MainViewModel.Instance.GetCategoryById(categoryId)?.Icon;
                        if (string.IsNullOrWhiteSpace(icon))
                        {
                            icon = "\uEA3A";
                        }

                        return icon;
                    }
                    else if (para == "Title")
                    {
                        string title = MainViewModel.Instance.GetCategoryById(categoryId)?.Title;
                        if (string.IsNullOrWhiteSpace(title))
                        {
                            _resourceLoader ??= new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
                            title = _resourceLoader.GetString("UnknownCategoryTitle");
                        }

                        return title;
                    }
                }
            }
            catch { }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}