CREATE TABLE [User].[UserCredentialRecovery]
(
	[Id]       BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[UserId]   BIGINT NOT NULL,
	[Question] NVARCHAR(MAX) NOT NULL,
	[Response] NVARCHAR(MAX) NOT NULL, 

    CONSTRAINT [FK_UserCredentialRecovery_User] FOREIGN KEY ([UserId]) REFERENCES [User].[User]([Id])
)

GO

CREATE INDEX [IX_UserId_Question] ON [User].[UserCredentialRecovery] ([UserId], [Question])
