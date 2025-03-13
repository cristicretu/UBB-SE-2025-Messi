using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Duo.Services
{
    public static class ValidationHelper
    {
        public static bool ValidateNotNullOrEmpty(string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"{parameterName} cannot be null or empty.");
            }
            return true;
        }

        public static bool ValidateRange<T>(T value, T min, T max, string parameterName) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, $"{parameterName} must be between {min} and {max}.");
            }
            return true;
        }

        public static bool ValidateCollectionNotEmpty<T>(ICollection<T> collection, string parameterName)
        {
            if (collection == null || collection.Count == 0)
            {
                throw new ArgumentException($"{parameterName} cannot be null or empty.");
            }
            return true;
        }

        public static bool ValidateCondition(bool condition, string errorMessage)
        {
            if (!condition)
            {
                throw new ArgumentException(errorMessage);
            }
            return true;
        }

        // Post validation
        public static bool ValidatePost(string content, string title = null)
        {
            // Content should not be null or empty
            ValidateNotNullOrEmpty(content, nameof(content));
            
            // Content should not exceed 4000 characters
            ValidateRange(content.Length, 1, 4000, "Post content length");
            
            // If title is provided, validate it
            if (title != null)
            {
                ValidateRange(title.Length, 1, 100, "Post title length");
            }
            
            return true;
        }

        // Comment validation
        public static bool ValidateComment(string content)
        {
            // Comment should not be null or empty
            ValidateNotNullOrEmpty(content, nameof(content));
            
            // Comment should not exceed 1000 characters
            ValidateRange(content.Length, 1, 1000, "Comment length");
            
            return true;
        }

        // Hashtag validation
        public static bool ValidateHashtag(string hashtag)
        {
            // Hashtag should not be null or empty
            ValidateNotNullOrEmpty(hashtag, nameof(hashtag));
            
            // Hashtag should not be empty after removing #
            ValidateCondition(!string.IsNullOrEmpty(tag), "Hashtag cannot be just a # symbol.");
            
            // Hashtag should only contain alphanumeric characters
            ValidateCondition(Regex.IsMatch(tag, @"^[a-zA-Z0-9]+$"), 
                "Hashtag can only contain letters and numbers.");
            
            // Hashtag should not exceed 30 characters
            ValidateRange(tag.Length, 1, 30, "Hashtag length");
            
            return true;
        }
    }
}