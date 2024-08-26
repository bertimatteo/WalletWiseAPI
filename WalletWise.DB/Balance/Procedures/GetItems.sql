CREATE PROCEDURE [Balance].[GetItems]
	@userId     BIGINT = NULL,
	@categoryId BIGINT = NULL,
	@startDate  DATETIME = NULL,
	@endDate    DATETIME = NULL
AS
	BEGIN

    SET NOCOUNT ON;

    SELECT [Balance].[Item].[Id],
	       [Balance].[Item].[UserId],
	       [CategoryId],
	       [Balance].[Item].[Description],
	       [Amount],
	       [Date]
      FROM [Balance].[Item]
	  INNER JOIN [Balance].[Category]
	  ON [CategoryId] = [Balance].[Category].[Id]
	  WHERE ([Balance].[Category].[UserId] = @userId     OR @userId IS NULL) AND
			([CategoryId] = @categoryId OR @categoryId IS NULL) AND
			([Date]      >= @startDate  OR @startDate IS NULL) AND
			([Date]      <= @endDate    OR @endDate IS NULL) AND
			[Balance].[Category].[IsDeleted] = 0

    RETURN @@ROWCOUNT

END
