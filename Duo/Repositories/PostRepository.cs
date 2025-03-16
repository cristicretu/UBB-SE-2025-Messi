    using System;
    using System.Data;
    using Microsoft.Data.SqlClient;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class PostRepository
    {
        private readonly DataLink dataLink;
    
        public PostRepository(DataLink dataLink)
        {
            this.dataLink = dataLink;
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
                int? result = dataLink.ExecuteScalar<int>("CreatePost", parameters);
                return result ?? 0;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }
    
        public void DeletePost(int id)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", id)
            };
            dataLink.ExecuteNonQuery("DeletePost", parameters);
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
            dataLink.ExecuteNonQuery("UpdatePost", parameters);
        }
    
        public Post? GetPostById(int id)
        {
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
            int offset = (page - 1) * pageSize;

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("CategoryID", categoryId),
                new SqlParameter("PageSize", pageSize), 
                new SqlParameter("Offset", offset)
            };

            DataTable dataTable = dataLink.ExecuteReader("GetPostsByCategory", parameters);
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

            return new Collection<Post>(posts);
        }

    }