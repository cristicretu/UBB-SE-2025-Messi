using Microsoft.UI.Xaml.Data;
using System;

namespace Duo.Converters
{
    public class LikesTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int count)
            {
                return $"{count} likes";
            }
            return "0 likes";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
} 