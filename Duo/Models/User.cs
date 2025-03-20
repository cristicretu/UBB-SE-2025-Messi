using System;

namespace Duo.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }

        public User(int userId, string username)
        {
            UserId = userId;
            Username = username;
        }

        public User(string username)
        {
            Username = username;
        }
    }
}