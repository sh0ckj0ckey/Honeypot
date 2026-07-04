using System;
using Microsoft.UI.Xaml.Data;

namespace Honeypot.Converters;

internal partial class IntToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        int intValue = value is int i ? i : 0;
        bool result = intValue > 0;

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
