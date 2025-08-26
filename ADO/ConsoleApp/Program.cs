using Microsoft.Data.SqlClient;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var connectionString = "Server=EPUALVIW077B\\SQLSERVER2022;Database=ado-fundamentals;Trusted_Connection=True;TrustServerCertificate=True;";

            var productsRepository = new ProductsRepository(connectionString);

            Console.WriteLine(productsRepository.GetProduct(2)?.Name);

            //productsRepository.UpdateProduct(
            //    1,
            //    new UpdateProductParams()
            //    {
            //        Name = "Namos",
            //        Description = "Dados",
            //        Weight = 1.2m,
            //        Height = 3.3m,
            //        Length = 2.2m,
            //        Width = 5.5m
            //    });

            //productsRepository.DeleteProduct(3);

            //ShowProducts(connectionString);

            productsRepository.CreateProduct(new CreateProductParams()
            {
                Name = "Namos",
                Description = "Dados",
                Weight = 1.2m,
                Height = 3.3m,
                Length = 2.2m,
                Width = 5.5m
            });
        }

        private static void ShowProducts(string connectionString)
        {
            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand command = new("SELECT * FROM Products;", connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Name"]}");
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
                reader.Close();
            }
        }
    }
}
