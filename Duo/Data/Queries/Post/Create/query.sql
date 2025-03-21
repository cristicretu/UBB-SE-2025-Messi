CREATE OR ALTER PROCEDURE CreatePost (
    @Title VARCHAR (20),
    @Description VARCHAR (4000),
    @UserID INT,
    @CategoryID INT,
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME,
    @LikeCount INT
) AS
BEGIN
    INSERT INTO Posts
        (Title, Description, UserID, CategoryID, CreatedAt, UpdatedAt, LikeCount)
    VALUES
        (@Title, @Description, @UserID, @CategoryID, @CreatedAt, @UpdatedAt, @LikeCount)
END