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
                        string icon = MainViewModel.Instance.GetCategoryById(categoryId)?.Icon;
                        if (string.IsNullOrWhiteSpace(icon))
                        {
                            icon = "\uE734";
                        }

                        return icon;
                    }
                    else if (para == "Title")
                    {
                        string title = MainViewModel.Instance.GetCategoryById(categoryId)?.Title;
                        if (string.IsNullOrWhiteSpace(title))
                        {
                            title = "未分类";
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