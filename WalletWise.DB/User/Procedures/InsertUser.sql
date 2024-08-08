CREATE PROCEDURE [User].[InsertUser]
    @username     NVARCHAR(255),  
    @email        NVARCHAR(255),       
    @passwordSalt NVARCHAR(255),
    @password NVARCHAR(255)
AS
	BEGIN

    DECLARE @err INT
    DECLARE @id  BIGINT
    
    DECLARE @transactionOwner   BIT

    SELECT @transactionOwner = IIF(@@TRANCOUNT = 0, 1, 0)
    
    IF @transactionOwner = 1 BEGIN TRANSACTION

    INSERT INTO [User].[User]
    (
        [Username],
	    [Email],
	    [PasswordSalt],
	    [Password] 
    )
    VALUES
    (
        @username,
        @email,
        @passwordSalt,
        @password
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
