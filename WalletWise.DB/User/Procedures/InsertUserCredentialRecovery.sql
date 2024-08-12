CREATE PROCEDURE [User].[InsertUserCredentialRecovery]
	@userId       BIGINT,  
    @question     NVARCHAR(MAX), 
    @responseSalt NVARCHAR(255),
    @response     NVARCHAR(MAX)
AS
	BEGIN

    DECLARE @err INT
    DECLARE @id  BIGINT
    
    DECLARE @transactionOwner   BIT

    SELECT @transactionOwner = IIF(@@TRANCOUNT = 0, 1, 0)
    
    IF @transactionOwner = 1 BEGIN TRANSACTION

    INSERT INTO [User].[UserCredentialRecovery]
    (
        [UserId],
        [Question],
        [ResponseSalt],
        [Response]
    )
    VALUES
    (
        @userId,
        @question,
        @responseSalt,
        @response
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