using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

public class CommentRepository : ICommentRepository
{
    private readonly DataLink _dataLink;

    public CommentRepository(DataLink dataLink)
    {
        _dataLink = dataLink ?? throw new ArgumentNullException(nameof(dataLink));
    }

    public int CreateComment(Comment comment)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@Content", comment.Content),
            new SqlParameter("@UserID", comment.UserID),
            new SqlParameter("@PostID", comment.PostID),
            new SqlParameter("@ParentCommentID", (object)comment.ParentCommentID ?? DBNull.Value),
            new SqlParameter("@Level", comment.Level)
        };

        // Modify CreateComment procedure to return SCOPE_IDENTITY()
        object result = _dataLink.ExecuteScalar("CreateComment", parameters);
        return Convert.ToInt32(result);
    }

    public Comment GetCommentById(int commentId)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@CommentID", commentId)
        };

        using (SqlDataReader reader = _dataLink.ExecuteReader("GetCommentById", parameters))
        {
            if (reader.Read())
            {
                return ReadComment(reader);
            }
            return null; // Return null if comment not found
        }
    }

    public IEnumerable<Comment> GetAllComments()
    {
        List<Comment> comments = new List<Comment>();
        using (SqlDataReader reader = _dataLink.ExecuteReader("GetAllComments", null))
        {
            while (reader.Read())
            {
                comments.Add(ReadComment(reader));
            }
        }
        return comments;
    }

    public void UpdateComment(int commentId, string newContent)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@CommentID", commentId),
            new SqlParameter("@NewContent", newContent)
        };

        _dataLink.ExecuteNonQuery("UpdateComment", parameters);
    }

    public void DeleteComment(int commentId)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@CommentID", commentId)
        };

        _dataLink.ExecuteNonQuery("DeleteComment", parameters);
    }

    public IEnumerable<Comment> GetReplies(int parentCommentId)
    {
        List<Comment> replies = new List<Comment>();
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@ParentCommentID", parentCommentId)
        };

        using (SqlDataReader reader = _dataLink.ExecuteReader("GetReplies", parameters))
        {
            while (reader.Read())
            {
                replies.Add(ReadComment(reader));
            }
        }
        return replies;
    }

    private Comment ReadComment(SqlDataReader reader)
    {
        return new Comment
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Content = reader.GetString(reader.GetOrdinal("Content")),
            UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
            PostID = reader.GetInt32(reader.GetOrdinal("PostID")),
            ParentCommentID = reader.IsDBNull(reader.GetOrdinal("ParentCommentID"))
                ? (int?)null
                : reader.GetInt32(reader.GetOrdinal("ParentCommentID")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            LikeCount = reader.GetInt32(reader.GetOrdinal("LikeCount")),
            Level = reader.GetInt32(reader.GetOrdinal("Level"))
        };
    }
}