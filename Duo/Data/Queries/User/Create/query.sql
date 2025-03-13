CREATE PROCEDURE CreateUser
    @Username NVARCHAR(30)
AS
BEGIN
    INSERT INTO Users (Username)
    VALUES (@Username);
END;
