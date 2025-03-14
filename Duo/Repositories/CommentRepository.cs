using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

public class CommentRepository
{
    private readonly DataLink _dataLink;

    public CommentRepository(DataLink dataLink)
    {
        _dataLink = dataLink;
    }

    public int CreateComment(Comment comment)
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
            return result ?? 0;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public void DeleteComment(int id)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@CommentID", id)
        };
        _dataLink.ExecuteNonQuery("DeleteComment", parameters);
    }

    public void UpdateComment(Comment comment)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@CommentID", comment.Id),
            new SqlParameter("@NewContent", comment.Content),
        };
        _dataLink.ExecuteNonQuery("UpdateComment", parameters);
    }

    public List<Comment> GetAllComments()
    {
        List<Comment> comments = new List<Comment>();
        var dataTable = _dataLink.ExecuteReader("GetAllComments");

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

    public List<Comment> GetReplies(int parentCommentId)
    {
        List<Comment> comments = new List<Comment>();
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@ParentCommentID", parentCommentId)
        };
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

}