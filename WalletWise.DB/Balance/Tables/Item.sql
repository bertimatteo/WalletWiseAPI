CREATE TABLE [Balance].[Item]
(
	[Id]          BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[UserId]      BIGINT NOT NULL,
	[CategoryId]  BIGINT NOT NULL,
	[Description] NVARCHAR(MAX) NOT NULL,
	[Amount]      FLOAT NOT NULL,
	[Date]        DATETIME NOT NULL
)
