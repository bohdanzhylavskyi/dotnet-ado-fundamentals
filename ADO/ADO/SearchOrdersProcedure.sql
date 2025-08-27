CREATE PROCEDURE [dbo].[SearchOrders]
	@Month int = null,
	@Year int = null,
	@Status nvarchar(30) = null,
	@ProductId int = null

AS
BEGIN
	SET NOCOUNT ON;

	SELECT * FROM Orders WHERE (@Month IS NULL OR MONTH(CreatedDate) = @Month)
	AND (@Year IS NULL OR YEAR(CreatedDate) = @Year)
	AND (@Status IS NULL OR Status = @Status)
	AND (@ProductId IS NULL OR ProductId = @ProductId);
END
