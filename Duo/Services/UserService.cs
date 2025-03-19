

using System;
using System.Collections.Generic;

public class UserService
{

    private readonly UserRepository userRepository;
    User currentUser = new User();
    public UserService(UserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    // Default constructor for testing
    public UserService() {}

    public void setUser(string name)
    {
        currentUser.Username = name;
    }

    public User GetCurrentUser()
    {
        if(currentUser == null)
        {
            throw new Exception("No user is currently logged in.");
        }
        return currentUser;
    }

    public int CreateUser(User user)
    {
        try
        {
            return userRepository.CreateUser(user);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    public User GetUserById(int id)
    {
        try
        {
            return userRepository.GetUserById(id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}


