using Duo.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Duo.Services
{
    public static class MockCommentService
    {
        public static List<MockComment> GetCommentsForPost(int postId)
        {
            // Create some mock comments
            var comments = new List<MockComment>
            {
                // Top-level comments (parent = -1)
                new MockComment
                {
                    Id = 1,
                    ParentId = -1,
                    User = "user1",
                    Description = "This is the first comment on this post. I really like the content!",
                    TreeLevel = 0,
                    Date = DateTime.Now.AddDays(-5)
                },
                new MockComment
                {
                    Id = 2,
                    ParentId = -1,
                    User = "user2",
                    Description = "Great post! I'm looking forward to more content like this.",
                    TreeLevel = 0,
                    Date = DateTime.Now.AddDays(-3)
                },
                
                // Second level comment (parent = 1)
                new MockComment
                {
                    Id = 3,
                    ParentId = 1,
                    User = "user3",
                    Description = "I agree with you! The content is very informative.",
                    TreeLevel = 1,
                    Date = DateTime.Now.AddDays(-2)
                },
                
                // Third level comments (parent = 3)
                new MockComment
                {
                    Id = 4,
                    ParentId = 3,
                    User = "user4",
                    Description = "This thread is getting interesting. Let me add to the discussion.",
                    TreeLevel = 2,
                    Date = DateTime.Now.AddDays(-1)
                },
                new MockComment
                {
                    Id = 5,
                    ParentId = 3,
                    User = "user5",
                    Description = "I have a different perspective, but I appreciate the discussion!",
                    TreeLevel = 2,
                    Date = DateTime.Now.AddHours(-12)
                }
            };

            return comments;
        }

        public static Dictionary<int, List<MockComment>> GroupCommentsByParent(List<MockComment> comments)
        {
            return comments
                .GroupBy(c => c.ParentId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }
    }
} 