CREATE PROCEDURE [User].[GetUserCredentialRecoveryByUserId]
	@userId   BIGINT
AS
	BEGIN

    SET NOCOUNT ON;

    SELECT [Id],
	       [UserId],
		   [Question],
	       [Response]
      FROM [User].[UserCredentialRecovery]
      WHERE [UserId] = @userId

    RETURN @@ROWCOUNT

END
