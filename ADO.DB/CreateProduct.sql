CREATE PROCEDURE [dbo].[CreateProduct]
	@Name varchar(20),
	@Description varchar(20),
	@Weight decimal(3, 2),
	@Height decimal(3, 2),
	@Width decimal(3, 2),
	@Length decimal(3, 2)
AS
	INSERT INTO Products (Name, Description, Weight, Height, Width, Length)
	VALUES (@Name, @Description, @Weight, @Height, @Width, @Length);
RETURN 0
