using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

public class PostService
{
    private readonly PostRepository postRepository;
    private SearchService searchService; 

    public PostService(PostRepository newPostRepository)
    {
        this.postRepository = newPostRepository;
        this.searchService = new SearchService();
    }

    public int CreatePost(Post post)
    {
        if (string.IsNullOrWhiteSpace(post.Title) || string.IsNullOrWhiteSpace(post.Description))
        {
            throw new ArgumentException("Title and Description cannot be empty.");
        }

        try
        {
            return postRepository.CreatePost(post);
        } 
        catch(Exception ex)
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
            postRepository.DeletePost(id);
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
            postRepository.UpdatePost(post);
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
            return postRepository.GetPostById(id);
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
            return postRepository.GetByCategory(categoryId, page, pageSize);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving posts for category {categoryId}: {ex.Message}");
        }
    }

    public List<Post> GetAllPosts()
    {
        return postRepository.GetAllPosts();
    }

    //public List<Post> GetPostsByUser(int userId, int page, int pageSize)
    //{
    //    if (userId <= 0 || page < 1 || pageSize < 1)
    //    {
    //        throw new ArgumentException("Invalid pagination parameters.");
    //    }
    //    try
    //    {
    //        return _postRepository.GetByUser(userId, page, pageSize);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new Exception($"Error retrieving posts for user {userId}: {ex.Message}");
    //    }
    //}

    //public bool LikePost(int postId)
    //{
    //    if (postId <= 0)
    //    {
    //        throw new ArgumentException("Invalid Post ID.");
    //    }
    //    try
    //    {
    //        return _postRepository.LikePost(postId);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new Exception($"Error liking post with ID {postId}: {ex.Message}");
    //    }
    //}

    //public List<Post> GetPostsByHashtag(string hashtag, int page, int pageSize)
    //{
    //    if (string.IsNullOrWhiteSpace(hashtag) || page < 1 || pageSize < 1)
    //    {
    //        throw new ArgumentException("Invalid pagination parameters.");
    //    }
    //    try
    //    {
    //        return _postRepository.GetByHashtag(hashtag, page, pageSize);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new Exception($"Error retrieving posts for hashtag {hashtag}: {ex.Message}");
    //    }
    //}

    public List<Post> SearchPostsByKeyword(string keyword)
    {   
        if (string.IsNullOrEmpty(keyword))
            return new List<Post>();

        List<string> allTitles = postRepository.GetAllPostTitles();
        List<string> matchingTitles = searchService.Search(keyword, allTitles, 0.6);

        List<Post> results = new List<Post>();
        foreach (string title in matchingTitles)
        {
            List<Post> postsWithTitle = postRepository.GetByTitle(title);
            results.AddRange(postsWithTitle);
        }

        return results;
    }

    public bool ValidatePostOwnership(int currentUserId, int currentPostId) {
        int? postUserId = postRepository.GetUserIdByPostId(currentPostId);

        return currentUserId == postUserId;
    }
}
