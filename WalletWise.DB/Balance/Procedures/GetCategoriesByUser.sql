CREATE PROCEDURE [Balance].[GetCategoriesByUser]
	@userId    BIGINT,
	@type      SMALLINT = NULL,
	@isDeleted BIT      = NULL
AS
	BEGIN

    SET NOCOUNT ON;

    SELECT [Id],
	       [UserId],
	       [Description],
		   [Type],
	       [Icon],
	       [ColorBackground],
		   [IsDeleted] 
      FROM [Balance].[Category]
      WHERE [UserId] = @userId AND 
			([Type]      = @type      OR @type IS NULL) AND
	        ([IsDeleted] = @isDeleted OR @isDeleted IS NULL) 

    RETURN @@ROWCOUNT

END
