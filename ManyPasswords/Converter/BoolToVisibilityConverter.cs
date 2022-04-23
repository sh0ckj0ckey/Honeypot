﻿using System;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace ManyPasswords.Converter
{
    internal class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null)
                {
                    if (parameter == null)
                    {
                        return bool.Parse(value.ToString()) ? Visibility.Visible : Visibility.Collapsed;
                    }
                    else if (parameter.ToString() == "-")
                    {
                        return bool.Parse(value.ToString()) ? Visibility.Collapsed : Visibility.Visible;
                    }
                }
            }
            catch { }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
