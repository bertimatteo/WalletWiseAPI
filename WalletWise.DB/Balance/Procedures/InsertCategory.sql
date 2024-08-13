CREATE PROCEDURE [Balance].[InsertCategory]
	@userId          BIGINT,  
    @description     NVARCHAR(MAX),       
    @icon            NVARCHAR(255),
    @colorBackground NVARCHAR(255),
    @isDeleted       BIT
AS
	BEGIN

    DECLARE @err INT
    DECLARE @id  BIGINT
    
    DECLARE @transactionOwner   BIT

    SELECT @transactionOwner = IIF(@@TRANCOUNT = 0, 1, 0)
    
    IF @transactionOwner = 1 BEGIN TRANSACTION

    INSERT INTO [Balance].[Category]
    (
        [UserId],
	    [Description],
	    [Icon],
	    [ColorBackground],
	    [IsDeleted]
    )
    VALUES
    (
        @userId,
        @description,       
        @icon,
        @colorBackground,
        @isDeleted
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