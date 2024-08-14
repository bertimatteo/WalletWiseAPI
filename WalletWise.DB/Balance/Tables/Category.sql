CREATE TABLE [Balance].[Category]
(
	[Id]              BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[UserId]          BIGINT NOT NULL,
	[Description]     NVARCHAR(MAX) NOT NULL,
	[Type]            SMALLINT NOT NULL,
	[Icon]            NVARCHAR(255) NOT NULL,
	[ColorBackground] NVARCHAR(255) NOT NULL,
	[IsDeleted]       BIT NOT NULL,

	CONSTRAINT [FK_Category_User] FOREIGN KEY ([UserId]) REFERENCES [User].[User]([Id])
)
