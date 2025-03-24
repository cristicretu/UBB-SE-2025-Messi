using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Duo.Helpers
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

        public static bool ValidatePost(string content, string? title = null)
        {

            ValidateNotNullOrEmpty(content, nameof(content));

            ValidateRange(content.Length, 1, 4000, "Post content length");

            if (title != null)
            {
                ValidateRange(title.Length, 1, 100, "Post title length");
            }

            return true;
        }

        public static bool ValidateComment(string content)
        {

            ValidateNotNullOrEmpty(content, nameof(content));

            ValidateRange(content.Length, 1, 1000, "Comment length");

            return true;
        }

        public static bool ValidateHashtag(string hashtag)
        {

            ValidateNotNullOrEmpty(hashtag, nameof(hashtag));

            ValidateCondition(!string.IsNullOrEmpty(hashtag), "Hashtag cannot be just a # symbol.");

            ValidateCondition(Regex.IsMatch(hashtag, @"^[a-zA-Z0-9]+$"), 
                "Hashtag can only contain letters and numbers.");

            ValidateRange(hashtag.Length, 1, 30, "Hashtag length");

            return true;
        }

        public static bool ValidateUsername(string username)
        {
            ValidateNotNullOrEmpty(username, nameof(username));

            ValidateRange(username.Length, 1, 30, "Username length");

            ValidateCondition(Regex.IsMatch(username, @"^[a-zA-Z0-9]+$"), 
                "Username can only contain letters and numbers.");

            ValidateCondition(!username.Contains(" "), "Username cannot contain spaces.");

            return true;
        }
    }
}