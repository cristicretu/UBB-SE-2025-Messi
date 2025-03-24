using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using Duo.Models;
using Duo.Services;
using Duo.Repositories;
using System.Diagnostics;
using System.Linq;

namespace Duo.Services
{
    public class PostService
    {
        private readonly PostRepository _postRepository;
        private readonly HashtagRepository _hashtagRepository;
        private readonly UserService _userService;
        private readonly SearchService _searchService;
        private const double FUZZY_SEARCH_SCORE_DEFAULT_THRESHOLD = 0.6;

        public PostService(PostRepository postRepository, HashtagRepository hashtagRepository, UserService userService, SearchService searchService)
        {
            _postRepository = postRepository;
            _hashtagRepository = hashtagRepository;
            _userService = userService;
            _searchService = searchService;
        }

        public int CreatePost(Post post)
        {
            if (string.IsNullOrWhiteSpace(post.Title) || string.IsNullOrWhiteSpace(post.Description))
            {
                throw new ArgumentException("Title and Description cannot be empty.");
            }

            try
            {
                return _postRepository.CreatePost(post);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating post: {ex.Message}");
            }
        }

        public void DeletePost(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid Post ID.");
            }

            try
            {
                _postRepository.DeletePost(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting post with ID {id}: {ex.Message}");
            }
        }

        public void UpdatePost(Post post)
        {
            if (post.Id <= 0)
            {
                throw new ArgumentException("Invalid Post ID.");
            }

            try
            {
                post.UpdatedAt = DateTime.UtcNow;
                _postRepository.UpdatePost(post);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating post with ID {post.Id}: {ex.Message}");
            }
        }

        public Post? GetPostById(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid Post ID.");
            }

            try
            {
                return _postRepository.GetPostById(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving post with ID {id}: {ex.Message}");
            }
        }

        public Collection<Post> GetPostsByCategory(int categoryId, int page, int pageSize)
        {
            if (categoryId <= 0 || page < 1 || pageSize < 1)
            {
                throw new ArgumentException("Invalid pagination parameters.");
            }

            try
            {
                return _postRepository.GetByCategory(categoryId, page, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving posts for category {categoryId}: {ex.Message}");
            }
        }

        public List<Post> GetPaginatedPosts(int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                throw new ArgumentException("Invalid pagination parameters.");
            }

            try
            {
                return _postRepository.GetPaginatedPosts(page, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving paginated posts: {ex.Message}");
            }
        }

        public int GetTotalPostCount()
        {
            try
            {
                return _postRepository.GetTotalPostCount();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving total post count: {ex.Message}");
            }
        }

        public int GetPostCountByCategory(int categoryId)
        {
            if (categoryId <= 0)
            {
                throw new ArgumentException("Invalid Category ID.");
            }

            try
            {
                return _postRepository.GetPostCountByCategory(categoryId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving post count for category {categoryId}: {ex.Message}");
            }
        }

        public int GetPostCountByHashtags(List<string> hashtags)
        {

            if (hashtags == null || hashtags.Count == 0)
            {
                return GetTotalPostCount();
            }

            hashtags = hashtags.Where(h => !string.IsNullOrWhiteSpace(h)).ToList();
            if (hashtags.Count == 0)
            {
                return GetTotalPostCount();
            }

            try
            {
                return _postRepository.GetPostCountByHashtags(hashtags);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving post count for hashtags: {ex.Message}");
            }
        }

        public List<Hashtag> GetAllHashtags()
        {
            try
            {
                return _hashtagRepository.GetAllHashtags();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving all hashtags: {ex.Message}");
            }
        }

        public List<Hashtag> GetHashtagsByCategory(int categoryId)
        {
            if (categoryId <= 0)
            {
                throw new ArgumentException("Invalid Category ID.");
            }

            try
            {
                return _hashtagRepository.GetHashtagsByCategory(categoryId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving hashtags for category {categoryId}: {ex.Message}");
            }
        }

        public List<Post> GetPostsByHashtags(List<string> hashtags, int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                throw new ArgumentException("Invalid pagination parameters.");
            }

            if (hashtags == null || hashtags.Count == 0)
            {
                return GetPaginatedPosts(page, pageSize);
            }

            hashtags = hashtags.Where(h => !string.IsNullOrWhiteSpace(h)).ToList();
            if (hashtags.Count == 0)
            {
                return GetPaginatedPosts(page, pageSize);
            }

            try
            {
                return _postRepository.GetByHashtags(hashtags, page, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving posts for hashtags: {ex.Message}");
            }
        }

        public bool ValidatePostOwnership(int currentUserId, int currentPostId)
        {
            int? postUserId = _postRepository.GetUserIdByPostId(currentPostId);
            return currentUserId == postUserId;
        }

        public List<Hashtag> GetHashtagsByPostId(int postId)
        {
            if (postId <= 0) throw new ArgumentException("Invalid Post ID.");

            try
            {
                Debug.WriteLine("We are here");
                return _hashtagRepository.GetHashtagsByPostId(postId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving hashtags for post with ID {postId}: {ex.Message}");
            }
        }

        public bool LikePost(int postId)
        {
            if (postId <= 0) throw new ArgumentException("Invalid Post ID.");

            try
            {
                var post = _postRepository.GetPostById(postId);
                if (post == null) throw new Exception("Post not found");

                post.LikeCount++;

                Debug.WriteLine($"Post like count: {post.LikeCount}");
                _postRepository.UpdatePost(post);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error liking post with ID {postId}: {ex.Message}");
            }
        }

        public bool AddHashtagToPost(int postId, string tagName, int userId)
        {
            if (postId <= 0)
            {
                System.Diagnostics.Debug.WriteLine($"AddHashtagToPost failed: Invalid Post ID {postId}");
                throw new ArgumentException("Invalid Post ID.");
            }
            
            if (string.IsNullOrWhiteSpace(tagName))
            {
                System.Diagnostics.Debug.WriteLine("AddHashtagToPost failed: Tag name cannot be empty");
                throw new ArgumentException("Tag name cannot be empty.");
            }
            
            if (userId <= 0)
            {
                System.Diagnostics.Debug.WriteLine($"AddHashtagToPost failed: Invalid User ID {userId}");
                throw new ArgumentException("Invalid User ID.");
            }

            try
            {
                // Validate the post exists
                var post = _postRepository.GetPostById(postId);
                if (post == null)
                {
                    System.Diagnostics.Debug.WriteLine($"AddHashtagToPost failed: Post with ID {postId} not found");
                    throw new Exception($"Post with ID {postId} not found");
                }
                
                if (_userService.GetCurrentUser().UserId != userId)
                {
                    System.Diagnostics.Debug.WriteLine($"AddHashtagToPost failed: User {userId} does not have permission");
                    throw new Exception("User does not have permission to add hashtags to this post.");
                }

                Hashtag? hashtag = null;
                hashtag = _hashtagRepository.GetHashtagByText(tagName);

                if (hashtag == null)
                {
                    System.Diagnostics.Debug.WriteLine($"AddHashtagToPost: Creating new hashtag '{tagName}'");
                    hashtag = _hashtagRepository.CreateHashtag(tagName);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"AddHashtagToPost: Using existing hashtag '{tagName}' with ID {hashtag.Id}");
                }

                bool result = _hashtagRepository.AddHashtagToPost(postId, hashtag.Id);
                System.Diagnostics.Debug.WriteLine($"AddHashtagToPost: Result={result} for post {postId}, hashtag {tagName}");
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddHashtagToPost error: {ex.Message}");
                throw new Exception($"Error adding hashtag to post with ID {postId}: {ex.Message}");
            }
        }

        public bool RemoveHashtagFromPost(int postId, int hashtagId, int userId)
        {
            if (postId <= 0) throw new ArgumentException("Invalid Post ID.");
            if (hashtagId <= 0) throw new ArgumentException("Invalid Hashtag ID.");
            if (userId <= 0) throw new ArgumentException("Invalid User ID.");

            try
            {
                if (_userService.GetCurrentUser().UserId != userId)
                    throw new Exception("User does not have permission to remove hashtags from this post.");

                return _hashtagRepository.RemoveHashtagFromPost(postId, hashtagId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error removing hashtag from post with ID {postId}: {ex.Message}");
            }
        }


        // New method to create a post with hashtags in a single operation
        public int CreatePostWithHashtags(Post post, List<string> hashtags, int userId)
        {
            if (string.IsNullOrWhiteSpace(post.Title) || string.IsNullOrWhiteSpace(post.Description))
            {
                System.Diagnostics.Debug.WriteLine("CreatePostWithHashtags: Title or Description is empty");
                throw new ArgumentException("Title and Description cannot be empty.");
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"CreatePostWithHashtags: Starting process for post with Title={post.Title}, User={userId}");
                System.Diagnostics.Debug.WriteLine($"CreatePostWithHashtags: Post has {hashtags?.Count ?? 0} hashtags to add");
                
                // First create the post
                System.Diagnostics.Debug.WriteLine("CreatePostWithHashtags: About to call _postRepository.CreatePost");
                int postId = _postRepository.CreatePost(post);
                
                // Check if post was created successfully
                if (postId <= 0)
                {
                    System.Diagnostics.Debug.WriteLine($"CreatePostWithHashtags: Failed to create post - Invalid ID returned: {postId}");
                    throw new Exception("Failed to create post: Invalid post ID returned from database");
                }
                
                System.Diagnostics.Debug.WriteLine($"CreatePostWithHashtags: Post created successfully with ID: {postId}");
                
                // Optionally verify the post exists in the database
                try
                {
                    var createdPost = _postRepository.GetPostById(postId);
                    if (createdPost != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"CreatePostWithHashtags: Verified post {postId} exists in database with Title={createdPost.Title}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"CreatePostWithHashtags: WARNING - Could not verify post {postId} exists!");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"CreatePostWithHashtags: Error verifying post exists: {ex.Message}");
                    // Continue anyway, as we have a post ID
                }
                
                // Now add hashtags if provided
                if (hashtags != null && hashtags.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"CreatePostWithHashtags: Adding {hashtags.Count} hashtags to post {postId}");
                    
                    foreach (var tagName in hashtags)
                    {
                        try
                        {
                            if (string.IsNullOrWhiteSpace(tagName))
                            {
                                System.Diagnostics.Debug.WriteLine("CreatePostWithHashtags: Skipping empty hashtag");
                                continue;
                            }
                                
                            System.Diagnostics.Debug.WriteLine($"CreatePostWithHashtags: Processing hashtag '{tagName}' for post {postId}");
                            
                            // Get or create the hashtag
                            Hashtag? hashtag = _hashtagRepository.GetHashtagByText(tagName);
                            if (hashtag == null)
                            {
                                System.Diagnostics.Debug.WriteLine($"CreatePostWithHashtags: Hashtag '{tagName}' not found, creating new one");
                                hashtag = _hashtagRepository.CreateHashtag(tagName);
                                System.Diagnostics.Debug.WriteLine($"CreatePostWithHashtags: Created hashtag with ID: {hashtag.Id}");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"CreatePostWithHashtags: Using existing hashtag '{tagName}' with ID {hashtag.Id}");
                            }
                            
                            // Associate hashtag with post
                            System.Diagnostics.Debug.WriteLine($"CreatePostWithHashtags: Adding hashtag {hashtag.Id} to post {postId}");
                            bool success = _hashtagRepository.AddHashtagToPost(postId, hashtag.Id);
                            System.Diagnostics.Debug.WriteLine($"CreatePostWithHashtags: Hashtag added successfully: {success}");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"CreatePostWithHashtags: Error adding hashtag '{tagName}' to post {postId}: {ex.Message}");
                            // Continue with other hashtags even if one fails
                        }
                    }
                }
                
                System.Diagnostics.Debug.WriteLine($"CreatePostWithHashtags: Process completed successfully for post {postId}");
                return postId;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CreatePostWithHashtags ERROR: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"CreatePostWithHashtags INNER ERROR: {ex.InnerException.Message}");
                }
                throw new Exception($"Error creating post with hashtags: {ex.Message}", ex);
            }
        }
    }
}