using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using Duo.Models;
using Duo.Data;

namespace Duo.Repositories
{
    public class UserRepository
    {
        private readonly DataLink dataLink;
        public UserRepository(DataLink dataLink)
        {
            this.dataLink = dataLink;
        }
        public int CreateUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(user.Username))
            {
                throw new ArgumentException("Username cannot be empty.");
            }

            var existingUser = GetUserByUsername(user.Username);
            if (existingUser != null)
            {
                return existingUser.UserId;
            }

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", user.Username),
            };
            try
            {
                int? result = dataLink.ExecuteScalar<int>("CreateUser", parameters);
                return result ?? 0;
            }
            catch (SqlException ex)
            {
                throw new Exception($"Database error: {ex.Message}");
            }
        }

        public User GetUserById(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid user ID.");
            }

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserID", id)
            };

            DataTable? dataTable = null;
            try
            {
                dataTable = dataLink.ExecuteReader("GetUserByID", parameters);
                if (dataTable.Rows.Count == 0)
                {
                    throw new Exception("User not found.");
                }
                var row = dataTable.Rows[0];
                return new User(
                    Convert.ToInt32(row[0]),
                    row[1]?.ToString() ?? string.Empty
                );
            }
            catch (SqlException ex)
            {
                throw new Exception($"Database error: {ex.Message}");
            }
            finally
            {
                dataTable?.Dispose();
            }
        }

        public User GetUserByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Invalid username.");
            }
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", username)
            };
            DataTable? dataTable = null;
            try
            {
                dataTable = dataLink.ExecuteReader("GetUserByUsername", parameters);
                if (dataTable.Rows.Count == 0)
                {
                    return null;
                }

                var row = dataTable.Rows[0];
                
                return new User(
                    Convert.ToInt32(row["userID"]),
                    row["username"]?.ToString() ?? string.Empty
                );
            }
            catch (SqlException ex)
            {
                throw new Exception($"Database error: {ex.Message}");
            }
            finally
            {
                dataTable?.Dispose();
            }
        }
    }
}

