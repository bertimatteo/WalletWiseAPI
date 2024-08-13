CREATE PROCEDURE [Balance].[UpdateCategory]
    @id              BIGINT,  
	@description     NVARCHAR(MAX),       
    @icon            NVARCHAR(255),
    @colorBackground NVARCHAR(255),
    @isDeleted       BIT
AS
	BEGIN

    DECLARE @err    INT
    DECLARE @count  INT
    
    DECLARE @transactionOwner   BIT

    SELECT @transactionOwner = IIF(@@TRANCOUNT = 0, 1, 0)
    
    IF @transactionOwner = 1 BEGIN TRANSACTION

    UPDATE [Balance].[Category]
       SET [Description]     = @description,
           [Icon]            = @icon,
           [ColorBackground] = @colorBackground,
           [IsDeleted]       = @isDeleted
     WHERE [Id] = @id

    SELECT @err = @@error, @count = @@ROWCOUNT IF @err <> 0 
    BEGIN 
        IF @transactionOwner = 1  ROLLBACK
        RETURN -1
    END

    IF @transactionOwner = 1 COMMIT  
    
    RETURN @count

END
