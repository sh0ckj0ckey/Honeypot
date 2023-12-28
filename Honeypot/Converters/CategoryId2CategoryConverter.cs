using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;
using Honeypot.ViewModels;

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
                    string para = parameter?.ToString();
                    if (para == "Icon")
                    {
                        return MainViewModel.Instance.GetCategoryById(categoryId)?.Icon;
                    }
                    else if (para == "Title")
                    {
                        return MainViewModel.Instance.GetCategoryById(categoryId)?.Title;
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