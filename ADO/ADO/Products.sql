CREATE TABLE [dbo].[Products]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(30) NOT NULL, 
    [Description] NVARCHAR(30) NOT NULL, 
    [Weight] DECIMAL(6, 3) NOT NULL, 
    [Height] DECIMAL(6, 3) NOT NULL, 
    [Width] DECIMAL(6, 3) NOT NULL, 
    [Length] DECIMAL(6, 3) NOT NULL
)
