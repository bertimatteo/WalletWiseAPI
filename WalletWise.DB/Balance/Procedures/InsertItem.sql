CREATE PROCEDURE [Balance].[InsertItem]
	@userId      BIGINT,  
    @categoryId  BIGINT,  
    @description NVARCHAR(MAX),  
    @amount      FLOAT,
    @date        DATETIME
AS
	BEGIN

    DECLARE @err INT
    DECLARE @id  BIGINT
    
    DECLARE @transactionOwner   BIT

    SELECT @transactionOwner = IIF(@@TRANCOUNT = 0, 1, 0)
    
    IF @transactionOwner = 1 BEGIN TRANSACTION

    INSERT INTO [Balance].[Item]
    (
        [UserId],
        [CategoryId],
	    [Description],
        [Amount],
        [Date]
    )
    VALUES
    (
        @userId,
        @categoryId,
        @description,  
        @amount,
        @date        
    )
    
    SELECT @err = @@error IF @err <> 0 
    BEGIN 
        IF @transactionOwner = 1 ROLLBACK
        RETURN -1 
    END

    SELECT @id = @@IDENTITY

    IF @transactionOwner = 1 COMMIT 
    
    RETURN @id

END
