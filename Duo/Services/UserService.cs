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

    public void setUser(string name)
    {
        currentUser.Username = name;
    }

    public string getUser()
    {
        return currentUser.Username;
    }

    public User GetCurrentUser()
    {
        return new User();
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

    public List<User> GetUsers()
    {
        return userRepository.GetUsers();
    }

}