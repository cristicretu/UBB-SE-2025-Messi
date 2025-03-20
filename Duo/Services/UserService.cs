using System;
using System.Collections.Generic;
using Duo.Models;
using Duo.Repositories;

namespace Duo.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private User _currentUser;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public void setUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be empty", nameof(username));
            }

            // Try to find existing user or create a new one
            var user = GetUserByUsername(username);
            if (user == null)
            {
                // Create new user
                user = new User(username);
                int userId = CreateUser(user);
                user = new User(userId, username);
            }

            _currentUser = user;
        }

        public User GetCurrentUser()
        {
            if (_currentUser == null)
            {
                throw new InvalidOperationException("No user is currently logged in.");
            }
            return _currentUser;
        }

        public int CreateUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(user.Username))
            {
                throw new ArgumentException("Username cannot be empty.", nameof(user));
            }

            try
            {
                return _userRepository.CreateUser(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create user: {ex.Message}", ex);
            }
        }

        public User GetUserById(int id)
        {
            try
            {
                return _userRepository.GetUserById(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get user by ID: {ex.Message}", ex);
            }
        }

        public User GetUserByUsername(string username)
        {
            try
            {
                // Use repository to find user
                return _userRepository.GetUserByUsername(username);
            }
            catch (Exception)
            {
                // Return null if user not found
                return null;
            }
        }
    }
}


