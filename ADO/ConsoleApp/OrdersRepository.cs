using Microsoft.Data.SqlClient;
using System.Data;

namespace ConsoleApp
{
    public enum OrderStatus
    {
        NotStarted,
        Loading,
        InProgress,
        Arrived,
        Unloading,
        Cancelled,
        Done
    }

    public struct CreateOrderParams
    {
        public required OrderStatus Status;
        public required DateTime CreatedDate;
        public required DateTime UpdatedDate;
        public required int ProductId;
    }

    public struct UpdateOrderParams
    {
        public required OrderStatus Status;
        public required DateTime CreatedDate;
        public required DateTime UpdatedDate;
        public required int ProductId;
    }

    public struct Order
    {
        public required int Id;
        public required OrderStatus Status;
        public required DateTime CreatedDate;
        public required DateTime UpdatedDate;
        public required int ProductId;
    }

    public class OrdersRepository
    {
        private string _connectionString;

        public OrdersRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public Order? GetOrder(int productId)
        {
            using (SqlConnection connection = new(_connectionString))
            {
                SqlCommand command = new("SELECT * FROM Orders WHERE Id = @Id;", connection);

                command.Parameters.AddWithValue("Id", productId);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                Order? result = null;

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result = new Order()
                        {
                            Id = reader.GetInt32("Id"),
                            Status = Enum.Parse<OrderStatus>(reader.GetString("Status")),
                            CreatedDate = reader.GetDateTime("CreatedDate"),
                            UpdatedDate = reader.GetDateTime("UpdatedDate"),
                            ProductId = reader.GetInt32("ProductId"),
                        };
                    }
                }

                reader.Close();

                return result;
            }
        }

        public int CreateOrder(CreateOrderParams parameters)
        {
            using (SqlConnection connection = new(this._connectionString))
            {
                SqlCommand command = new(
                    "INSERT INTO Orders (Status, CreatedDate, UpdatedDate, ProductId) " +
                    "OUTPUT INSERTED.Id " +
                    "VALUES (@Status, @CreatedDate, @UpdatedDate, @ProductId);", connection);

                command.Parameters.AddWithValue("Status", parameters.Status.ToString());
                command.Parameters.AddWithValue("CreatedDate", parameters.CreatedDate);
                command.Parameters.AddWithValue("UpdatedDate", parameters.UpdatedDate);
                command.Parameters.AddWithValue("ProductId", parameters.ProductId);

                connection.Open();
                return (int) command.ExecuteScalar();
            }
        }

        public void UpdateOrder(int orderId, UpdateOrderParams parameters)
        {
            using (SqlConnection connection = new(this._connectionString))
            {
                SqlCommand command = new(
                "UPDATE Orders SET Status=@Status, CreatedDate=@CreatedDate, UpdatedDate=@UpdatedDate, ProductId=@ProductId" +
                " WHERE Id=@Id", connection);

                command.Parameters.AddWithValue("Status", parameters.Status.ToString());
                command.Parameters.AddWithValue("CreatedDate", parameters.CreatedDate);
                command.Parameters.AddWithValue("UpdatedDate", parameters.UpdatedDate);
                command.Parameters.AddWithValue("ProductId", parameters.ProductId);
                command.Parameters.AddWithValue("Id", orderId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void DeleteOrder(int orderId)
        {
            using (SqlConnection connection = new(this._connectionString))
            {
                SqlCommand command = new(
                    "DELETE FROM Orders WHERE Id = @Id;", connection);

                command.Parameters.AddWithValue("Id", orderId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<Order> ListOrders()
        {
            using (SqlConnection connection = new(_connectionString))
            {
                SqlCommand command = new("SELECT * FROM Orders;", connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                var result = new List<Order>();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result.Add(new Order()
                        {
                            Id = reader.GetInt32("Id"),
                            Status = Enum.Parse<OrderStatus>(reader.GetString("Status")),
                            CreatedDate = reader.GetDateTime("CreatedDate"),
                            UpdatedDate = reader.GetDateTime("UpdatedDate"),
                            ProductId = reader.GetInt32("ProductId"),
                        });
                    }
                }

                reader.Close();

                return result;
            }
        }
    }
}
