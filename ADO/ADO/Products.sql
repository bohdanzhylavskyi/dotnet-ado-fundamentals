CREATE TABLE [dbo].[Products]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NCHAR(20) NOT NULL, 
    [Description] NCHAR(20) NOT NULL, 
    [Weight] DECIMAL(3, 2) NOT NULL, 
    [Height] DECIMAL(3, 2) NOT NULL, 
    [Width] DECIMAL(3, 2) NOT NULL, 
    [Length] DECIMAL(3, 2) NOT NULL
)
