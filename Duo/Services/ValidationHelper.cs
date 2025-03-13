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

        // Username validation
        public static bool ValidateUsername(string username)
        {
            // Username should not be null or empty
            ValidateNotNullOrEmpty(username, nameof(username));
            
            // Username should be between 3 and 20 characters
            ValidateRange(username.Length, 3, 20, "Username length");
            
            // Username should only contain alphanumeric characters and underscores
            ValidateCondition(Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"), 
                "Username can only contain letters, numbers, and underscores.");
            
            return true;
        }

        // Password validation
        public static bool ValidatePassword(string password)
        {
            // Password should not be null or empty
            ValidateNotNullOrEmpty(password, nameof(password));
            
            // Password should be at least 8 characters
            ValidateCondition(password.Length >= 8, 
                "Password must be at least 8 characters long.");
            
            // Password should contain at least one uppercase letter, one lowercase letter, and one digit
            ValidateCondition(Regex.IsMatch(password, @"[A-Z]"), 
                "Password must contain at least one uppercase letter.");
            ValidateCondition(Regex.IsMatch(password, @"[a-z]"), 
                "Password must contain at least one lowercase letter.");
            ValidateCondition(Regex.IsMatch(password, @"[0-9]"), 
                "Password must contain at least one digit.");
            
            return true;
        }

        // Email validation
        public static bool ValidateEmail(string email)
        {
            // Email should not be null or empty
            ValidateNotNullOrEmpty(email, nameof(email));
            
            // Email should match a basic email pattern
            ValidateCondition(Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"), 
                "Email format is invalid.");
            
            return true;
        }

        // Post validation
        public static bool ValidatePost(string content, string title = null)
        {
            // Content should not be null or empty
            ValidateNotNullOrEmpty(content, nameof(content));
            
            // Content should not exceed 5000 characters
            ValidateRange(content.Length, 1, 5000, "Post content length");
            
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
            
            // Remove # if present at the beginning
            string tag = hashtag.StartsWith("#") ? hashtag.Substring(1) : hashtag;
            
            // Hashtag should not be empty after removing #
            ValidateCondition(!string.IsNullOrEmpty(tag), "Hashtag cannot be just a # symbol.");
            
            // Hashtag should only contain alphanumeric characters
            ValidateCondition(Regex.IsMatch(tag, @"^[a-zA-Z0-9]+$"), 
                "Hashtag can only contain letters and numbers.");
            
            // Hashtag should not exceed 30 characters
            ValidateRange(tag.Length, 1, 30, "Hashtag length");
            
            return true;
        }

        // Login validation
        public static bool ValidateLogin(string username, string password)
        {
            // Validate username and password
            ValidateUsername(username);
            ValidatePassword(password);
            
            return true;
        }
    }
}