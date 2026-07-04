using System;
using Microsoft.UI.Xaml.Data;

namespace Honeypot.Converters;

internal partial class StringToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        bool result = !string.IsNullOrWhiteSpace(value?.ToString());

        if (parameter?.ToString() == "!")
        {
            result = !result;
        }

        return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}