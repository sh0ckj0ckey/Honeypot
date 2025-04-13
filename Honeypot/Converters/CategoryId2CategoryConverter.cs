using System;
using Honeypot.Helpers;
using Microsoft.UI.Xaml.Data;

namespace Honeypot.Converters
{
    class CategoryId2CategoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                string id = value?.ToString();
                if (int.TryParse(id, out int categoryId))
                {
                    return PasswordsGetter.GetCategoryById(categoryId);
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