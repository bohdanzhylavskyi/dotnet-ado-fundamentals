CREATE TABLE [dbo].[Orders]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CreatedDate] DATETIME NOT NULL, 
    [UpdatedDate] DATETIME NOT NULL, 
    [ProductId] INT NOT NULL, 
    [Status] NVARCHAR(30) NOT NULL CHECK ([Status] IN('NotStarted', 'Loading', 'InProgress', 'Arrived', 'Unloading', 'Cancelled', 'Done')), 
    CONSTRAINT [FK_Orders_Products] FOREIGN KEY ([ProductId]) REFERENCES [Products]([Id])
)
