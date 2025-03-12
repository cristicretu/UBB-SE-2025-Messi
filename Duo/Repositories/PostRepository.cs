using System;
using Microsoft.Data.SqlClient;


public class PostRepository
{
    private readonly DataLink _dataLink;

    public PostRepository(DataLink dataLink)
    {
        _dataLink = dataLink;
    }

    public int CreatePost(Post post)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@Title", post.Title),
            new SqlParameter("@Description", post.Description),
            new SqlParameter("@UserID", post.UserID),
            new SqlParameter("@CategoryID", post.CategoryID),
            new SqlParameter("@CreatedAt", post.CreatedAt),
            new SqlParameter("@UpdatedAt", post.UpdatedAt),
            new SqlParameter("@LikeCount", post.LikeCount)
        };

        try
        {
            _dataLink.ExecuteScalar("CreatePost", parameters);
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
        return 0;
    }

    public void DeletePost(int id)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@Id", id)
        };

        _dataLink.ExecuteNonQuery("DeletePost", parameters);
    }

    public void UpdatePost(Post post)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@Id", post.Id),
            new SqlParameter("@Title", post.Title),
            new SqlParameter("@Description", post.Description),
            new SqlParameter("@UserID", post.UserID),
            new SqlParameter("@CategoryID", post.CategoryID),
            new SqlParameter("@UpdatedAt", post.UpdatedAt),
            new SqlParameter("@LikeCount", post.LikeCount)
        };

        _dataLink.ExecuteNonQuery("UpdatePost", parameters);
    }

    public Post GetPostById(int id)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@Id", id)
        };

        using (SqlDataReader reader = _dataLink.ExecuteReader("GetPostById", parameters))
        {
            if (reader.Read())
            {
                return new Post
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.GetString(2),
                    UserID = reader.GetInt32(3),
                    CategoryID = reader.GetInt32(4),
                    CreatedAt = reader.GetDateTime(5),
                    UpdatedAt = reader.GetDateTime(6),
                    LikeCount = reader.GetInt32(7)
                };
            }
            return null; 
        }
    }
}