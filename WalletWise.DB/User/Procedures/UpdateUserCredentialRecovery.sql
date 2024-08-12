CREATE PROCEDURE [User].[UpdateUserCredentialRecovery]
	@userId       BIGINT,      
    @question     NVARCHAR(MAX),
    @responseSalt NVARCHAR(255),
    @response     NVARCHAR(MAX)
AS
	BEGIN

    DECLARE @err    INT
    DECLARE @count  INT
    
    DECLARE @transactionOwner   BIT

    SELECT @transactionOwner = IIF(@@TRANCOUNT = 0, 1, 0)
    
    IF @transactionOwner = 1 BEGIN TRANSACTION

    UPDATE [User].[UserCredentialRecovery]
       SET [Question]     = @question,
           [ResponseSalt] = @responseSalt,
           [Response]     = @response
     WHERE [UserId] = @userId

    SELECT @err = @@error, @count = @@ROWCOUNT IF @err <> 0 
    BEGIN 
        IF @transactionOwner = 1  ROLLBACK
        RETURN -1
    END

    IF @transactionOwner = 1 COMMIT  
    
    RETURN @count

END