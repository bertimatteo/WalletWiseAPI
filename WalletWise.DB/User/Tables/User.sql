CREATE TABLE [User].[User]
(
	[Id]           BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[Username]     NVARCHAR(255) NOT NULL,
	[Email]        NVARCHAR(255) NOT NULL,
	[PasswordSalt] NVARCHAR(255) NOT NULL,
	[Password]     NVARCHAR(255) NOT NULL
)

GO

CREATE INDEX [IX_Username_Password] ON [User].[User] ([Username], [Password])