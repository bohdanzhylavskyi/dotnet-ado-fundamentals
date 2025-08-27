CREATE PROCEDURE [dbo].[DeleteOrders]
	@Month int = null,
	@Year int = null,
	@Status nvarchar(30) = null,
	@ProductId int = null

AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		BEGIN TRANSACTION;

		DELETE FROM Orders WHERE (@Month IS NOT NULL AND MONTH(CreatedDate) = @Month)
		OR (@Year IS NOT NULL AND YEAR(CreatedDate) = @Year)
		OR (@Status IS NOT NULL AND Status = @Status)
		OR (@ProductId IS NOT NULL AND ProductId = @ProductId);

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION;
		THROW;
	END CATCH
END
