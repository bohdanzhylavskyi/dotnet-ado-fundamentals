using Microsoft.Data.SqlClient;
using System.Data;

namespace ADO.Lib
{
    public struct CreateProductParams
    {
        public required string Name;
        public required string Description;
        public required decimal Weight;
        public required decimal Height;
        public required decimal Width;
        public required decimal Length;
    }

    public struct UpdateProductParams
    {
        public required string Name;
        public required string Description;
        public required decimal Weight;
        public required decimal Height;
        public required decimal Width;
        public required decimal Length;
    }

    public struct Product
    {
        public required int Id;
        public required string Name;
        public required string Description;
        public required decimal Weight;
        public required decimal Height;
        public required decimal Width;
        public required decimal Length;
    }

    public class ProductsRepository
    {
        private string _connectionString;

        public ProductsRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public Product? GetProduct(int productId)
        {
            using (SqlConnection connection = new(_connectionString))
            {
                SqlCommand command = new("SELECT * FROM Products WHERE Id = @Id;", connection);

                command.Parameters.AddWithValue("Id", productId);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                Product? result = null;

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result = new Product()
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            Description = reader.GetString("Description"),
                            Weight = reader.GetDecimal("Weight"),
                            Height = reader.GetDecimal("Height"),
                            Width = reader.GetDecimal("Width"),
                            Length = reader.GetDecimal("Length"),
                        };
                    }
                }

                reader.Close();

                return result;
            }
        }

        public int CreateProduct(CreateProductParams parameters)
        {
            using (SqlConnection connection = new(this._connectionString))
            {
                SqlCommand command = new(
                    "INSERT INTO Products (Name, Description, Weight, Height, Width, Length) " +
                    "OUTPUT INSERTED.Id " +
                    "VALUES (@Name, @Description, @Weight, @Height, @Width, @Length);", connection);

                command.Parameters.AddWithValue("Name", parameters.Name);
                command.Parameters.AddWithValue("Description", parameters.Description);
                command.Parameters.AddWithValue("Weight", parameters.Weight);
                command.Parameters.AddWithValue("Height", parameters.Height);
                command.Parameters.AddWithValue("Width", parameters.Width);
                command.Parameters.AddWithValue("Length", parameters.Length);

                connection.Open();

                return (int) command.ExecuteScalar();
            }
        }

        public void UpdateProduct(int productId, UpdateProductParams parameters)
        {
            using (SqlConnection connection = new(this._connectionString)) {
                SqlCommand command = new(
                "UPDATE Products SET Name=@Name, Description=@Description, Weight=@Weight, Height=@Height, Width=@Width, Length=@Length" +
                " WHERE Id=@Id", connection);

                command.Parameters.AddWithValue("Name", parameters.Name);
                command.Parameters.AddWithValue("Description", parameters.Description);
                command.Parameters.AddWithValue("Weight", parameters.Weight);
                command.Parameters.AddWithValue("Height", parameters.Height);
                command.Parameters.AddWithValue("Width", parameters.Width);
                command.Parameters.AddWithValue("Length", parameters.Length);
                command.Parameters.AddWithValue("Id", productId);
                
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void DeleteProduct(int productId)
        {
            using (SqlConnection connection = new(this._connectionString))
            {
                SqlCommand command = new(
                    "DELETE FROM Products WHERE Id = @Id;", connection);

                command.Parameters.AddWithValue("Id", productId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<Product> ListProducts()
        {
            using (SqlConnection connection = new(_connectionString))
            {
                SqlCommand command = new("SELECT * FROM Products;", connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                var result = new List<Product>();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result.Add(new Product()
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            Description = reader.GetString("Description"),
                            Weight = reader.GetDecimal("Weight"),
                            Height = reader.GetDecimal("Height"),
                            Width = reader.GetDecimal("Width"),
                            Length = reader.GetDecimal("Length"),
                        });
                    }
                }

                reader.Close();

                return result;
            }
        }
    }
}
