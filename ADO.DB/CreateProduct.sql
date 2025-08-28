CREATE PROCEDURE [dbo].[CreateProduct]
	@Name varchar(20),
	@Description varchar(20),
	@Weight decimal(6, 3),
	@Height decimal(6, 3),
	@Width decimal(6, 3),
	@Length decimal(6, 3)
AS
	INSERT INTO Products (Name, Description, Weight, Height, Width, Length)
	VALUES (@Name, @Description, @Weight, @Height, @Width, @Length);
RETURN 0
