﻿using System;
using Microsoft.UI.Xaml.Data;

namespace Honeypot.Converters
{
    internal class IsExpanded2OpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null)
                {
                    return bool.Parse(value?.ToString() ?? "False") ? 0.3 : 0.7;
                }
            }
            catch { }
            return 0.7;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
