using System;
using System.Collections.Generic;

namespace Duo.Models
{
    public class Post
    {
        public Post()
        {
            Title = string.Empty;
            Description = string.Empty;
            Hashtags = new List<string>();
        }
        
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int UserID { get; set; }
        public int CategoryID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int LikeCount { get; set; }
        
        // Added properties to resolve compilation errors
        public string Content { get => Description; set => Description = value; } // Map Content to Description
        public string Username { get; set; } // For user display name
        public string Date { get; set; } // For formatted date display
        public List<string> Hashtags { get; set; } = new List<string>(); // For storing hashtag strings
    }
}