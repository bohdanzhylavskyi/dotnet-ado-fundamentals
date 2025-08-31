using Microsoft.Data.SqlClient;
using System.Data;

namespace ADO.Lib
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

    public class Order
    {
        public int Id { get; set; }
        public required OrderStatus Status;
        public required DateTime CreatedDate;
        public required DateTime UpdatedDate;
        public required int ProductId;
    }

    public struct OrderFilters
    {
        public int? Month { get; init; } = null;
        public int? Year { get; init; } = null;
        public OrderStatus? Status { get; init; } = null;
        public int? ProductId { get; init; } = null;
        public OrderFilters() {}
    }

    public class OrdersRepository
    {
        private string _connectionString;

        public OrdersRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public void CreateOrder(Order order)
        {
            using (SqlConnection connection = new(this._connectionString))
            {
                SqlCommand command = new(
                    "INSERT INTO Orders (Status, CreatedDate, UpdatedDate, ProductId) " +
                    "OUTPUT INSERTED.Id " +
                    "VALUES (@Status, @CreatedDate, @UpdatedDate, @ProductId);", connection);

                command.Parameters.AddWithValue("Status", order.Status.ToString());
                command.Parameters.AddWithValue("CreatedDate", order.CreatedDate);
                command.Parameters.AddWithValue("UpdatedDate", order.UpdatedDate);
                command.Parameters.AddWithValue("ProductId", order.ProductId);

                connection.Open();

                var id = (int)command.ExecuteScalar();

                order.Id = id;
            }
        }


        public Order? GetOrder(int orderId)
        {
            using (SqlConnection connection = new(_connectionString))
            {
                SqlCommand command = new("SELECT * FROM Orders WHERE Id = @Id;", connection);

                command.Parameters.AddWithValue("Id", orderId);

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

        public void UpdateOrder(Order order)
        {
            using (SqlConnection connection = new(this._connectionString))
            {
                SqlCommand command = new(
                "UPDATE Orders SET Status=@Status, CreatedDate=@CreatedDate, UpdatedDate=@UpdatedDate, ProductId=@ProductId" +
                " WHERE Id=@Id", connection);

                command.Parameters.AddWithValue("Status", order.Status.ToString());
                command.Parameters.AddWithValue("CreatedDate", order.CreatedDate);
                command.Parameters.AddWithValue("UpdatedDate", order.UpdatedDate);
                command.Parameters.AddWithValue("ProductId", order.ProductId);
                command.Parameters.AddWithValue("Id", order.Id);

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

        public List<Order> SearchOrdersByMonth(int month)
        {
            var filters = new OrderFilters()
            {
                Month = month
            };

            return SearchOrders(filters);
        }

        public List<Order> SearchOrdersByYear(int year)
        {
            var filters = new OrderFilters()
            {
                Year = year
            };

            return SearchOrders(filters);
        }

        public List<Order> SearchOrdersByStatus(OrderStatus status)
        {
            var filters = new OrderFilters()
            {
                Status = status
            };

            return SearchOrders(filters);
        }

        public List<Order> SearchOrdersByProduct(int productId)
        {
            var filters = new OrderFilters()
            {
                ProductId = productId
            };

            return SearchOrders(filters);
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

        public void DeleteOrdersByMonth(int month)
        {
            var filters = new OrderFilters()
            {
                Month = month
            };

            DeleteOrders(filters);
        }

        public void DeleteOrdersByYear(int year)
        {
            var filters = new OrderFilters()
            {
                Year = year
            };

            DeleteOrders(filters);
        }

        public void DeleteOrdersByStatus(OrderStatus status)
        {
            var filters = new OrderFilters()
            {
                Status = status
            };

            DeleteOrders(filters);
        }

        public void DeleteOrdersByProduct(int productId)
        {
            var filters = new OrderFilters()
            {
                ProductId = productId
            };

            DeleteOrders(filters);
        }

        private List<Order> SearchOrders(OrderFilters filters)
        {
            using (SqlConnection connection = new(_connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand()
                {
                    CommandText = "SearchOrders",
                    CommandType = CommandType.StoredProcedure,
                    Connection = connection
                };

                if (filters.Month is not null) command.Parameters.AddWithValue("Month", filters.Month);
                if (filters.Year is not null) command.Parameters.AddWithValue("Year", filters.Year);
                if (filters.Status is not null) command.Parameters.AddWithValue("Status", filters.Status.ToString());
                if (filters.ProductId is not null) command.Parameters.AddWithValue("ProductId", filters.ProductId);


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

        private void DeleteOrders(OrderFilters filters)
        {
            using (SqlConnection connection = new(this._connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand()
                {
                    CommandText = "DeleteOrders",
                    CommandType = CommandType.StoredProcedure,
                    Connection = connection
                };

                if (filters.Month is not null) command.Parameters.AddWithValue("Month", filters.Month);
                if (filters.Year is not null) command.Parameters.AddWithValue("Year", filters.Year);
                if (filters.Status is not null) command.Parameters.AddWithValue("Status", filters.Status.ToString());
                if (filters.ProductId is not null) command.Parameters.AddWithValue("ProductId", filters.ProductId);

                command.ExecuteNonQuery();
            }
        }
    }
}
