CREATE PROCEDURE [User].[UpdateUser]
	@username     NVARCHAR(255),      
    @passwordSalt NVARCHAR(255),
    @password     NVARCHAR(255)
AS
	BEGIN

    DECLARE @err    INT
    DECLARE @count  INT
    
    DECLARE @transactionOwner   BIT

    SELECT @transactionOwner = IIF(@@TRANCOUNT = 0, 1, 0)
    
    IF @transactionOwner = 1 BEGIN TRANSACTION

    UPDATE [User].[User]
       SET [PasswordSalt] = @passwordSalt,
           [Password]     = @password
     WHERE [Username] = @username

    SELECT @err = @@error, @count = @@ROWCOUNT IF @err <> 0 
    BEGIN 
        IF @transactionOwner = 1  ROLLBACK
        RETURN -1
    END

    IF @transactionOwner = 1 COMMIT  
    
    RETURN @count

END