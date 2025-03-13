CREATE PROCEDURE GetReplies
    @ParentCommentID INT
AS
BEGIN
    SELECT * FROM Comments WHERE ParentCommentID = @ParentCommentID ORDER BY CreatedAt ASC;
END;