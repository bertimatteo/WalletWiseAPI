CREATE PROCEDURE [Balance].[DeleteItem]
	@id BIGINT
AS
	BEGIN

    DECLARE @err    INT
    DECLARE @count  INT
    
    DECLARE @transactionOwner   BIT

    SELECT @transactionOwner = IIF(@@TRANCOUNT = 0, 1, 0)
    
    IF @transactionOwner = 1 BEGIN TRANSACTION

    DELETE FROM [Balance].[Item]
     WHERE [Id] = @id

    SELECT @err = @@error, @count = @@ROWCOUNT IF @err <> 0 
    BEGIN 
        IF @transactionOwner = 1  ROLLBACK
        RETURN -1
    END

    IF @transactionOwner = 1 COMMIT  
    
    RETURN @count

END
