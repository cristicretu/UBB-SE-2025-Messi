using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;

namespace Duo.Helpers
{
    /// <summary>
    /// Extension methods for UI elements
    /// </summary>
    public static class UIElementExtensions
    {
        /// <summary>
        /// Gets all the realized children of an ItemsRepeater
        /// </summary>
        /// <param name="itemsRepeater">The ItemsRepeater to get children from</param>
        /// <returns>Collection of UIElements that are children of the ItemsRepeater</returns>
        public static IEnumerable<UIElement> GetChildren(this ItemsRepeater itemsRepeater)
        {
            for (int i = 0; i < itemsRepeater.ItemsSourceView.Count; i++)
            {
                var element = itemsRepeater.TryGetElement(i);
                if (element != null)
                {
                    yield return element;
                }
            }
        }
    }
} 