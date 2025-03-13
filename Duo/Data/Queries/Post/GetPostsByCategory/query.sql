CREATE OR ALTER GetPostsByCategory (
    @CategoryID INT,
    @PageSize INT,
    @Offset INT
) AS
BEGIN 
    SELECT Id, Title, Description, UserID, CategoryID, CreatedAt, UpdatedAt, LikeCount
    FROM Posts
    WHERE CategoryID = @CategoryID
    ORDER BY CreatedAt DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY
END