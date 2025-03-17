using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class PostService
{
    private readonly PostRepository _postRepository;

    public PostService(PostRepository postRepository)
    {
        _postRepository = postRepository;
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

    public List<Post> GetPostsByCategory(int categoryId, int page, int pageSize)
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

}
