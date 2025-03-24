using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using Duo.Models;
using Duo.Data;

namespace Duo.Repositories
{
    public class CommentRepository
    {
        private readonly DataLink _dataLink;

        public CommentRepository(DataLink dataLink)
        {
            _dataLink = dataLink ?? throw new ArgumentNullException(nameof(dataLink));
        }

        public Comment GetCommentById(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid comment ID", nameof(id));

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@CommentID", id)
            };

            DataTable? dataTable = null;
            try
            {
                dataTable = _dataLink.ExecuteReader("GetCommentByID", parameters);
                if (dataTable.Rows.Count == 0)
                    throw new Exception("Comment not found");

                if (dataTable.Columns.Count < 8)
                    throw new Exception("Invalid data structure returned from database");

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
            finally
            {
                dataTable?.Dispose();
            }
        }

        public List<Comment> GetCommentsByPostId(int postId)
        {
            if (postId <= 0) throw new ArgumentException("Invalid post ID", nameof(postId));

            List<Comment> comments = new List<Comment>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@PostID", postId)
            };

            System.Diagnostics.Debug.WriteLine($"CommentRepository: Getting comments for post ID {postId}");
            
            DataTable? dataTable = null;
            try
            {
                dataTable = _dataLink.ExecuteReader("GetCommentsByPostID", parameters);
                System.Diagnostics.Debug.WriteLine($"CommentRepository: Query executed, retrieved {dataTable?.Rows?.Count ?? 0} rows");
                
                if (dataTable.Columns.Count < 8)
                {
                    System.Diagnostics.Debug.WriteLine($"CommentRepository: Invalid data structure, only {dataTable.Columns.Count} columns returned");
                    throw new Exception("Invalid data structure returned from database");
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    try
                    {
                        int commentId = Convert.ToInt32(row[0]);
                        Comment comment = new Comment(
                            commentId,
                            row[1]?.ToString() ?? string.Empty,
                            Convert.ToInt32(row[2]),
                            Convert.ToInt32(row[3]),
                            row[4] == DBNull.Value ? null : Convert.ToInt32(row[4]),
                            Convert.ToDateTime(row[5]),
                            Convert.ToInt32(row[7]),
                            Convert.ToInt32(row[6])
                        );
                        comments.Add(comment);
                        System.Diagnostics.Debug.WriteLine($"CommentRepository: Added comment ID {commentId} to result list");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"CommentRepository: Error processing comment row: {ex.Message}");
                    }
                }
                System.Diagnostics.Debug.WriteLine($"CommentRepository: Returning {comments.Count} comments");
                return comments;
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"CommentRepository: SQL error getting comments: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"CommentRepository: SQL error number: {ex.Number}");
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CommentRepository: General error getting comments: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"CommentRepository: Stack trace: {ex.StackTrace}");
                throw;
            }
            finally
            {
                dataTable?.Dispose();
            }
        }

        public int CreateComment(Comment comment)
        {
            if (comment == null) throw new ArgumentNullException(nameof(comment));
            if (string.IsNullOrEmpty(comment.Content)) throw new ArgumentException("Content cannot be empty");
            if (comment.UserId <= 0) throw new ArgumentException("Invalid user ID");
            if (comment.PostId <= 0) throw new ArgumentException("Invalid post ID");

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Content", comment.Content),
                new SqlParameter("@UserID", comment.UserId),
                new SqlParameter("@PostID", comment.PostId),
                new SqlParameter("@ParentCommentID", (object?)comment.ParentCommentId ?? DBNull.Value),
                new SqlParameter("@Level", comment.Level)
            };

            try
            {
                int? result = _dataLink.ExecuteScalar<int>("CreateComment", parameters);
                if (result == null)
                    throw new Exception("Failed to create comment");

                comment.Id = result.Value;
                return result.Value;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool DeleteComment(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid comment ID", nameof(id));

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

        public List<Comment> GetRepliesByCommentId(int parentCommentId)
        {
            if (parentCommentId <= 0) throw new ArgumentException("Invalid parent comment ID", nameof(parentCommentId));

            List<Comment> comments = new List<Comment>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ParentCommentID", parentCommentId)
            };

            DataTable? dataTable = null;
            try
            {
                dataTable = _dataLink.ExecuteReader("GetReplies", parameters);
                if (dataTable.Columns.Count < 8)
                    throw new Exception("Invalid data structure returned from database");

                foreach (DataRow row in dataTable.Rows)
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
            finally
            {
                dataTable?.Dispose();
            }
        }

        public bool UpdateComment(Comment comment)
        {
            if (comment == null) throw new ArgumentNullException(nameof(comment));
            if (comment.Id <= 0) throw new ArgumentException("Invalid comment ID");
            if (string.IsNullOrEmpty(comment.Content)) throw new ArgumentException("Content cannot be empty");

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

        public bool IncrementLikeCount(int commentId)
        {
            if (commentId <= 0) throw new ArgumentException("Invalid comment ID", nameof(commentId));

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@CommentID", commentId)
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

        public int GetCommentsCountForPost(int postId)
        {
            if (postId <= 0) throw new ArgumentException("Invalid post ID", nameof(postId));

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@PostID", postId)
            };

            try
            {
                int? result = _dataLink.ExecuteScalar<int>("GetCommentsCountForPost", parameters);
                if (result == null)
                    throw new Exception("Failed to get comment count");
                return result.Value;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
