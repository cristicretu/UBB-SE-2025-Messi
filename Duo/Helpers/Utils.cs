using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Duo.Helpers
{
    public static class Utils
    {
        public static string ConvertKebabCaseToTitleCase(string kebabCase)
        {
            if (string.IsNullOrEmpty(kebabCase))
                return string.Empty;
                
            string[] words = kebabCase.Split('-');
            
            for (int i = 0; i < words.Length; i++)
            {
                if (!string.IsNullOrEmpty(words[i]))
                {
                    words[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i].ToLower());
                }
            }
            
            return string.Join(" ", words);
        }
    }
}