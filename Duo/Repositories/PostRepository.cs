using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using Windows.Storage.Streams;

public class PostRepository
{
    private readonly DataLink dataLink;

    public PostRepository(DataLink dataLink)
    {
        this.dataLink = dataLink;
    }
    public int CreatePost(Post post)
    {
        if (post == null)
        {
            throw new ArgumentNullException(nameof(post), "Post cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(post.Title) || string.IsNullOrWhiteSpace(post.Description))
        {
            throw new ArgumentException("Title and Description cannot be empty.");
        }

        if (post.UserID <= 0 || post.CategoryID <= 0)
        {
            throw new ArgumentException("Invalid UserID or CategoryID.");
        }

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
            int? result = dataLink.ExecuteScalar<int>("CreatePost", parameters);
            return result ?? 0;
        }
        catch (SqlException ex)
        {
            throw new Exception($"Database error: {ex.Message}");
        }
    }

    public void DeletePost(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid post ID.");
        }

        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@Id", id)
        };
        dataLink.ExecuteNonQuery("DeletePost", parameters);
    }

    public void UpdatePost(Post post)
    {
        if (post == null)
        {
            throw new ArgumentNullException(nameof(post), "Post cannot be null.");
        }

        if (post.Id <= 0)
        {
            throw new ArgumentException("Invalid post ID.");
        }

        if (string.IsNullOrWhiteSpace(post.Title) || string.IsNullOrWhiteSpace(post.Description))
        {
            throw new ArgumentException("Title and Description cannot be empty.");
        }

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
        dataLink.ExecuteNonQuery("UpdatePost", parameters);
    }

    public Post? GetPostById(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid post ID.");
        }

        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@Id", id)
        };

        DataTable dataTable = dataLink.ExecuteReader("GetPostById", parameters);

        if (dataTable.Rows.Count > 0)
        {
            DataRow row = dataTable.Rows[0];
            return new Post
            {
                Id = Convert.ToInt32(row["Id"]),
                Title = Convert.ToString(row["Title"]) ?? string.Empty,
                Description = Convert.ToString(row["Description"]) ?? string.Empty,
                UserID = Convert.ToInt32(row["UserID"]),
                CategoryID = Convert.ToInt32(row["CategoryID"]),
                CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                UpdatedAt = Convert.ToDateTime(row["UpdatedAt"]),
                LikeCount = Convert.ToInt32(row["LikeCount"])
            };
        }

        return null;
    }

    public Collection<Post> GetByCategory(int categoryId, int page, int pageSize)
    {
        if (categoryId <= 0)
        {
            throw new ArgumentException("Invalid category ID.");
        }
        if (page <= 0)
        {
            throw new ArgumentException("Page number must be greater than 0.");
        }
        if (pageSize <= 0)
        {
            throw new ArgumentException("Page size must be greater than 0.");
        }

        int offset = (page - 1) * pageSize;

        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("CategoryID", categoryId),
            new SqlParameter("PageSize", pageSize),
            new SqlParameter("Offset", offset)
        };

        DataTable dataTable = dataLink.ExecuteReader("GetPostsByCategory", parameters);
        List<Post> posts = new List<Post>();

        foreach (DataRow row in dataTable.Rows)
        {
            posts.Add(new Post
            {
                Id = Convert.ToInt32(row["Id"]),
                Title = Convert.ToString(row["Title"]) ?? string.Empty,
                Description = Convert.ToString(row["Description"]) ?? string.Empty,
                UserID = Convert.ToInt32(row["UserID"]),
                CategoryID = Convert.ToInt32(row["CategoryID"]),
                CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                UpdatedAt = Convert.ToDateTime(row["UpdatedAt"]),
                LikeCount = Convert.ToInt32(row["LikeCount"])
            });
        }

        return new Collection<Post>(posts);
    }

    public List<Post> GetAllPosts()
    {
        DataTable? dataTable = null;
        try
        {
            dataTable = dataLink.ExecuteReader("GetAllPosts");
            List<Post> posts = new List<Post>();

            foreach (DataRow row in dataTable.Rows)
            {
                posts.Add(new Post
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Title = Convert.ToString(row["Title"]) ?? string.Empty,
                    Description = Convert.ToString(row["Description"]) ?? string.Empty,
                    UserID = Convert.ToInt32(row["UserID"]),
                    CategoryID = Convert.ToInt32(row["CategoryID"]),
                    CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                    UpdatedAt = Convert.ToDateTime(row["UpdatedAt"]),
                    LikeCount = Convert.ToInt32(row["LikeCount"])
                });
            }
            return posts;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            dataTable?.Dispose();
        }
    }

    public List<string> GetAllPostTitles()
    {
        var titles = new List<string>();

        try 
        {
            DataTable dataTable = dataLink.ExecuteReader("GetAllPostTitles", null);

            foreach (DataRow row in dataTable.Rows)
            {
                titles.Add(row["Title"].ToString());
            }

            return titles;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting post titles: {ex.Message}");
            return titles;
        }
    }

    public List<Post> GetByTitle(string title)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@Title", title)
        };
        
        try
        {
            DataTable dataTable = dataLink.ExecuteReader("GetPostsByTitle", parameters);
            List<Post> posts = new List<Post>();
            
            foreach(DataRow row in dataTable.Rows)
            {
                posts.Add(new Post
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Title = Convert.ToString(row["Title"]) ?? string.Empty,
                    Description = Convert.ToString(row["Description"]) ?? string.Empty,
                    UserID = Convert.ToInt32(row["UserID"]),
                    CategoryID = Convert.ToInt32(row["CategoryID"]),
                    CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                    UpdatedAt = Convert.ToDateTime(row["UpdatedAt"]),
                    LikeCount = Convert.ToInt32(row["LikeCount"])
                });
            }
            
            return posts;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting posts by title: {ex.Message}");
            return new List<Post>();
        }
    }

    public int? GetUserIdByPostId(int PostId)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@PostId", PostId)
        };

        try
        {
            DataTable dataTable = dataLink.ExecuteReader("GetUserIdByPostId", parameters); 
            
            if (dataTable.Rows.Count > 0) {
                DataRow row = dataTable.Rows[0];

                int postId = Convert.ToInt32(row["UserId"]);
                return postId; 
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new Exception($"GetUserIdByPostId exception: {ex.Message}");
        }
    }

    public List<Post> GetByUser(int userId, int page, int pageSize)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@UserID", userId),
            new SqlParameter("@PageSize", pageSize),
            new SqlParameter("@Offset", page)
        };

        try
        {
            DataTable dataTable = dataLink.ExecuteReader("GetPostsByUser");

            if(dataTable.Rows.Count > 0)
            {
                List<Post> posts = new List<Post>();
                
                foreach (DataRow row in dataTable.Rows)
                {
                    posts.Add(new Post
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Title = Convert.ToString(row["Title"]) ?? string.Empty,
                        Description = Convert.ToString(row["Description"]) ?? string.Empty,
                        UserID = Convert.ToInt32(row["UserID"]),
                        CategoryID = Convert.ToInt32(row["CategoryID"]),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                        UpdatedAt = Convert.ToDateTime(row["UpdatedAt"]),
                        LikeCount = Convert.ToInt32(row["LikeCount"])
                    });
                }

                return posts;
            }

            return null;
        } 
        catch (Exception ex) 
        {
            throw new Exception($"GetByUser: {ex.Message}");
        }
    }

    public List<Post> GetByHashtags(List<string> hashtags, int page, int pageSize)
    {
        string hashtagsString = string.Join(",", hashtags);
        
        int offset = (page - 1) * pageSize;
        
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@hashtags", hashtagsString),
            new SqlParameter("@PageSize", pageSize),
            new SqlParameter("@Offset", offset)
        };
        
        try
        {
            DataTable dataTable = dataLink.ExecuteReader("GetByHashtags", parameters);
            if(dataTable != null && dataTable.Rows.Count > 0)
            {
                List<Post> posts = new List<Post>();
                foreach (DataRow row in dataTable.Rows)
                {
                    posts.Add(new Post
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Title = Convert.ToString(row["Title"]) ?? string.Empty,
                        Description = Convert.ToString(row["Description"]) ?? string.Empty,
                        UserID = Convert.ToInt32(row["UserID"]),
                        CategoryID = Convert.ToInt32(row["CategoryID"]),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                        UpdatedAt = Convert.ToDateTime(row["UpdatedAt"]),
                        LikeCount = Convert.ToInt32(row["LikeCount"])
                    });
                }
                return posts;
            }
            return new List<Post>();
        }
        catch (Exception ex)
        {
            throw new Exception($"GetByHashtags: {ex.Message}");
        }
    }
}