CREATE PROCEDURE [Balance].[GetItemById]
	@id BIGINT
AS
	BEGIN

    SET NOCOUNT ON;

    SELECT [Id],
	       [UserId],
	       [CategoryId],
	       [Description],
	       [Amount],
	       [Date]
      FROM [Balance].[Item]
	  WHERE [Id] = @id

    RETURN @@ROWCOUNT

END
