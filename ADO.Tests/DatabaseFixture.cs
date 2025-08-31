using ADO.Lib;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Dac;

public class DatabaseFixture : IDisposable
{
    public string ConnectionString { get; }

    private string ServerConnectionString;
    private string TestDbName;

    public Product product1;
    public Product product2;
    public Product product3;

    public DatabaseFixture()
    {
        var folder = Directory.GetCurrentDirectory();

        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        
        ServerConnectionString = config.GetConnectionString("ServerConnection");
        TestDbName = config.GetSection("TestDbName").Value;

        ConnectionString = $"{ServerConnectionString}Database={TestDbName};";

        DeployDatabase();

        this.product1 = new Product()
        {
            Id = 0,
            Name = "Laptop Dell XPS 13",
            Description = "Ultra-portable laptop",
            Weight = 1.25m,
            Height = 1.5m,
            Length = 30.2m,
            Width = 20.0m
        };

        this.product2 = new Product()
        {
            Id = 1,
            Name = "Wooden Chair",
            Description = "Oak dining chair with cushion",
            Weight = 6.5m,
            Height = 95.0m,
            Length = 45.0m,
            Width = 50.0m
        };

        this.product3 = new Product()
        {
            Id = 2,
            Name = "Samsung 55'' QLED TV",
            Description = "Smart TV, 4K UHD, HDR10+",
            Weight = 17.3m,
            Height = 71.0m,
            Length = 123.0m,
            Width = 5.5m
        };

    }

    public void SeedProducts()
    {
        this.AddProduct(this.product1);
        this.AddProduct(this.product2);
        this.AddProduct(this.product3);
    }

    private void AddProduct(Product product)
    {
        using (SqlConnection connection = new(ConnectionString))
        {
            SqlCommand command = new(
                    "INSERT INTO Products (Name, Description, Weight, Height, Width, Length) " +
                    "OUTPUT INSERTED.Id " +
                    "VALUES (@Name, @Description, @Weight, @Height, @Width, @Length);", connection);

            command.Parameters.AddWithValue("Name", product.Name);
            command.Parameters.AddWithValue("Description", product.Description);
            command.Parameters.AddWithValue("Weight", product.Weight);
            command.Parameters.AddWithValue("Height", product.Height);
            command.Parameters.AddWithValue("Width", product.Width);
            command.Parameters.AddWithValue("Length", product.Length);

            connection.Open();

            var id = (int)command.ExecuteScalar();

            product.Id = id;
        }
    }


    private void DeployDatabase()
    {
        string dacpacPath = @"..\..\..\..\ADO.DB\bin\Debug\ADO.DB.dacpac";

        using var dacpac = DacPackage.Load(dacpacPath);
        var dacServices = new DacServices(ServerConnectionString);

        dacServices.Deploy(dacpac, TestDbName, upgradeExisting: true, options: new DacDeployOptions
        {
            BlockOnPossibleDataLoss = false,
            DropObjectsNotInSource = true
        });
    }

    public void Dispose()
    {
        using var connection = new SqlConnection(ServerConnectionString);
        connection.Open();
        new SqlCommand($"ALTER DATABASE [{TestDbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{TestDbName}];", connection)
            .ExecuteNonQuery();
    }
}

[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture> {}