CREATE PROCEDURE [dbo].[CreateOrder]
	@Status varchar(20) ,
	@CreatedDate datetime,
	@UpdatedDate datetime,
	@ProductId int
AS
	INSERT INTO Orders (Status, CreatedDate, UpdatedDate, ProductId)
	VALUES (@Status, @CreatedDate, @UpdatedDate, @ProductId);
RETURN 0
