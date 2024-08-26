CREATE TABLE [Balance].[Item]
(
	[Id]          BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[UserId]      BIGINT NOT NULL,
	[CategoryId]  BIGINT NOT NULL,
	[Description] NVARCHAR(MAX) NOT NULL,
	[Amount]      FLOAT NOT NULL,
	[Date]        DATETIME NOT NULL,

	CONSTRAINT [FK_Item_User] FOREIGN KEY ([UserId]) REFERENCES [User].[User]([Id]),
	CONSTRAINT [FK_Item_Category] FOREIGN KEY ([CategoryId]) REFERENCES [Balance].[Category]([Id])
)
