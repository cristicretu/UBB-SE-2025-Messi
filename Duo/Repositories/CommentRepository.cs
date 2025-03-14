using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Data.SqlClient;

public class CommentRepository
{
    private readonly DataLink _dataLink;

    public CommentRepository(DataLink dataLink)
    {
        _dataLink = dataLink;
    }

public Comment GetCommentById(int id)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
        new SqlParameter("@CommentID", id)
        };

        try
        {
            var dataTable = _dataLink.ExecuteReader("GetCommentByID", parameters);
            var row = dataTable.Rows[0];

            return new Comment(
                Convert.ToInt32(row[0]),
                row[1]?.ToString() ?? string.Empty,
                Convert.ToInt32(row[2]),
                Convert.ToInt32(row[3]),
                row[4] == DBNull.Value ? 0 : Convert.ToInt32(row[4]),
                Convert.ToDateTime(row[5]),
                Convert.ToInt32(row[6]),
                Convert.ToInt32(row[7])
            );
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public List<Comment> GetCommentsByPostId(int postId)
    {
        List<Comment> comments = new List<Comment>();
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@PostID", postId)
        };
        try
        {
            var dataTable = _dataLink.ExecuteReader("GetCommentsByPostID", parameters);
            foreach (System.Data.DataRow row in dataTable.Rows)
            {
                Comment comment = new Comment(
                    Convert.ToInt32(row[0]),
                    row[1]?.ToString() ?? string.Empty,
                    Convert.ToInt32(row[2]),
                    Convert.ToInt32(row[3]),
                    row[4] == DBNull.Value ? 0 : Convert.ToInt32(row[4]),
                    Convert.ToDateTime(row[5]),
                    Convert.ToInt32(row[6]),
                    Convert.ToInt32(row[7])
                );
                comments.Add(comment);
            }
            return comments;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

public Comment CreateComment(Comment comment)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@Content", comment.Content),
            new SqlParameter("@UserID", comment.UserId),
            new SqlParameter("@PostID", comment.PostId),
            new SqlParameter("@ParentCommentID", comment.ParentCommentId),
            new SqlParameter("@Level", comment.Level)
        };
        try
        {
            int? result = _dataLink.ExecuteScalar<int>("CreateComment", parameters);
            comment.Id = result.Value;
            return comment;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public bool DeleteComment(int id)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@CommentID", id)
        };
        try
        {
            _dataLink.ExecuteNonQuery("DeleteComment", parameters);
            return true;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public bool UpdateComment(Comment comment)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@CommentID", comment.Id),
            new SqlParameter("@NewContent", comment.Content),
        };
        try
        {
            _dataLink.ExecuteNonQuery("UpdateComment", parameters);
            return true;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public List<Comment> GetReplies(int parentCommentId)
    {
        List<Comment> comments = new List<Comment>();
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@ParentCommentID", parentCommentId)
        };

        try
        {
            var dataTable = _dataLink.ExecuteReader("GetReplies", parameters);

            foreach (System.Data.DataRow row in dataTable.Rows)
            {
                Comment comment = new Comment(
                    Convert.ToInt32(row[0]),
                    row[1]?.ToString() ?? string.Empty,
                    Convert.ToInt32(row[2]),
                    Convert.ToInt32(row[3]),
                    row[4] == DBNull.Value ? 0 : Convert.ToInt32(row[4]),
                    Convert.ToDateTime(row[5]),
                    Convert.ToInt32(row[6]),
                    Convert.ToInt32(row[7])
                );
                comments.Add(comment);
            }
            return comments;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public bool IncrementLikeCount(int parentCommentId)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@CommentID", parentCommentId)
        };
        try
        {
            _dataLink.ExecuteNonQuery("IncrementLikeCount", parameters);
            return true;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

}