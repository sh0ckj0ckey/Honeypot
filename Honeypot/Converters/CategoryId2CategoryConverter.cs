using System;
using Honeypot.Helpers;
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
                    var category = PasswordsGetter.GetCategoryById(categoryId);
                    if (category is not null)
                    {
                        return category;
                    }
                }

                _resourceLoader ??= new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader();
                return new Models.CategoryModel
                {
                    Id = -1,
                    Icon = "\uEA3A",
                    Title = _resourceLoader.GetString("UnknownCategoryTitle"),
                };
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