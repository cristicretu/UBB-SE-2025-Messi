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


        public static (bool IsValid, string ErrorMessage) ValidatePostTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return (false, "Title cannot be empty.");
            }
            
            if (title.Length < 3)
            {
                return (false, "Title should be at least 3 characters long.");
            }
            
            if (title.Length > 50)
            {
                return (false, "Title cannot exceed 50 characters.");
            }
            
            return (true, string.Empty);
        }

        public static (bool IsValid, string ErrorMessage) ValidatePostContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return (false, "Content cannot be empty.");
            }
            
            if (content.Length < 10)
            {
                return (false, "Content should be at least 10 characters long.");
            }
            
            if (content.Length > 4000)
            {
                return (false, "Content cannot exceed 4000 characters.");
            }
            
            return (true, string.Empty);
        }

        public static (bool IsValid, string ErrorMessage) ValidateHashtagInput(string hashtag)
        {
            if (string.IsNullOrWhiteSpace(hashtag))
            {
                return (true, string.Empty); 
            }
            
            string cleanHashtag = hashtag.StartsWith("#") ? hashtag.Substring(1) : hashtag;
            
            if (string.IsNullOrWhiteSpace(cleanHashtag))
            {
                return (false, "Hashtag cannot be just a # symbol.");
            }
            
            if (!Regex.IsMatch(cleanHashtag, @"^[a-zA-Z0-9]+$"))
            {
                return (false, "Hashtag can only contain letters and numbers.");
            }
            
            if (cleanHashtag.Length > 30)
            {
                return (false, "Hashtag cannot exceed 30 characters.");
            }
            
            return (true, string.Empty);
        }

    }
}