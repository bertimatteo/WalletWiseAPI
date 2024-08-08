CREATE PROCEDURE [User].[GetUserByUsername]
	@username NVARCHAR(255)
AS
	BEGIN

    SET NOCOUNT ON;

    SELECT [Id],
	       [Username],
	       [Email],
	       [PasswordSalt],
	       [Password] 
      FROM [User].[User]
      WHERE [Username] = @username

    RETURN @@ROWCOUNT

END