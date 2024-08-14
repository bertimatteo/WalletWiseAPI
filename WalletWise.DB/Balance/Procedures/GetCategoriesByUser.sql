CREATE PROCEDURE [Balance].[GetCategoriesByUser]
	@userId    BIGINT,
	@isDeleted BIT
AS
	BEGIN

    SET NOCOUNT ON;

    SELECT [Id],
	       [UserId],
	       [Description],
	       [Icon],
	       [ColorBackground],
		   [IsDeleted] 
      FROM [Balance].[Category]
      WHERE [UserId] = @userId AND [IsDeleted] = @isDeleted

    RETURN @@ROWCOUNT

END
