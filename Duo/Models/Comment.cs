using System;

public class Comment
{
    private int _id;
    private string _content;
    private DateTime _createdAt = DateTime.Now;
    private int _postId;
    private int _userId;
    private int? _parentCommentId;
    private int _likeCount = 0;
    private int _level = 1;

    public Comment(int id, string content, int userId, int postId, int? parentCommentId, DateTime createdAt, int likeCount, int level)
    {
        _id = id;
        _content = content;
        _userId = userId;
        _postId = postId;
        _parentCommentId = parentCommentId;
        _createdAt = createdAt;
        _likeCount = likeCount;
        _level = level;
    }

    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }

    public int PostId
    {
        get { return _postId; }
        set { _postId = value; }
    }

    public int UserId
    {
        get { return _userId; }
        set { _userId = value; }
    }

    public int? ParentCommentId
    {
        get { return _parentCommentId; }
        set { _parentCommentId = value; }
    }


    public string Content
    {
        get { return _content; }
        set { _content = value; }
    }

    public DateTime CreatedAt
    {
        get { return _createdAt; }
        set { _createdAt = value; }
    }

    public int LikeCount
    {
        get { return _likeCount; }
        set { _likeCount = value; }
    }

    public int Level
    {
        get { return _level; }
        set { _level = value; }
    }
}