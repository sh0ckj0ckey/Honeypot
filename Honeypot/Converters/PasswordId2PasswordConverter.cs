using System;
using Honeypot.Helpers;
using Microsoft.UI.Xaml.Data;

namespace Honeypot.Converters
{
    class PasswordId2PasswordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                string id = value?.ToString();
                if (int.TryParse(id, out int passwordId))
                {
                    return PasswordsGetter.GetPasswordById(passwordId);
                }
            }
            catch { }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}