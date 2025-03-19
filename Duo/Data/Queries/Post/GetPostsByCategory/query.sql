CREATE PROCEDURE GetPostsByCategory
  @Offset INT,
    @PageSize INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        p.Id, 
        p.Title, 
        p.Description, 
        p.UserID, 
        p.CategoryID, 
        p.CreatedAt, 
        p.UpdatedAt, 
        p.LikeCount, 
        u.Username, 
        c.CategoryName
    FROM Posts p
    JOIN Users u ON p.UserID = u.Id  
    JOIN Categories c ON p.CategoryID = c.Id 
    WHERE p.CategoryID = @CategoryID
    ORDER BY p.CreatedAt DESC  
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;