using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

public class PostService
{
    private readonly PostRepository _postRepository;
    private CancellationTokenSource _debounceTokenSource;
    private readonly SemaphoreSlim _searchLock = new SemaphoreSlim(1, 1);

    public PostService(PostRepository postRepository)
    {
        _postRepository = postRepository;
        _debounceTokenSource = new CancellationTokenSource();
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
        }catch(Exception ex)
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

    public List<Post> GetAllPosts()
    {
        return _postRepository.GetAllPosts();
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


    public async Task<List<Post>> SearchPostsAsync(string query, int page, int pageSize, int delay = 300)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new List<Post>();
        }

        if (page < 1 || pageSize < 1)
        {
            throw new ArgumentException("Invalid pagination parameters.");
        }

        // Cancel any previous search operation
        _debounceTokenSource?.Cancel();
        _debounceTokenSource = new CancellationTokenSource();
        var token = _debounceTokenSource.Token;

        try
        {
            // Wait for the debounce delay
            await Task.Delay(delay, token);

            // Check if this operation was canceled
            token.ThrowIfCancellationRequested();

            // Ensure only one search operation executes at a time
            await _searchLock.WaitAsync(token);
            try
            {
                // Perform the search
                var result = _postRepository.SearchPosts(query, page, pageSize);
                return result;
            }
            finally
            {
                _searchLock.Release();
            }
        }
        catch (OperationCanceledException)
        {
            // Return empty list if operation was canceled
            return new List<Post>();
        }
    }

    public async Task<List<Post>> GetAllPostsAsync()
    {
        return await Task.Run(() => _postRepository.GetAllPosts());
    }

    public void Dispose()
    {
        _debounceTokenSource?.Dispose();
        _searchLock.Dispose();
    }



}
