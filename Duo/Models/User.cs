using System;

public class User
{
    public int Id { get; init; }  // `init` makes it immutable after initialization
    public string Username { get; set; } = string.Empty;  // Prevents null issues

    public User(int id, string username)
    {
        Id = id;
        Username = username ?? throw new ArgumentNullException(nameof(username), "Username cannot be null.");
    }

    public User()
    {
    }
}
