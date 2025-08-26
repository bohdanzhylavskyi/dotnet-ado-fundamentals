using ConsoleApp;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Dac;

public class DatabaseFixture : IDisposable
{
    public string ConnectionString { get; }

    private string ServerConnectionString = "Server=EPUALVIW077B\\SQLSERVER2022;Integrated Security=true;TrustServerCertificate=True;";

    private const string TestDbName = "test-ado-fundamentals";

    public Product product1;
    public Product product2;
    public Product product3;

    public DatabaseFixture()
    {
        ConnectionString = $"{ServerConnectionString}Database={TestDbName};";

        DeployDatabase();

        this.product1 = new Product()
        {
            Id = 0,
            Name = "Laptop Dell XPS 13",
            Description = "Ultra-portable laptop",
            Weight = 1.25m,    // kg
            Height = 1.5m,     // cm
            Length = 30.2m,    // cm
            Width = 20.0m      // cm
        };

        this.product2 = new Product()
        {
            Id = 1,
            Name = "Wooden Chair",
            Description = "Oak dining chair with cushion",
            Weight = 6.5m,     // kg
            Height = 95.0m,    // cm
            Length = 45.0m,    // cm
            Width = 50.0m      // cm
        };

        this.product3 = new Product()
        {
            Id = 2,
            Name = "Samsung 55'' QLED TV",
            Description = "Smart TV, 4K UHD, HDR10+",
            Weight = 17.3m,   // kg
            Height = 71.0m,   // cm
            Length = 123.0m,  // cm
            Width = 5.5m      // cm (panel thickness)
        };

    }

    public void AddProducts()
    {
        var product1Id = this.AddProduct(this.product1);
        var product2Id = this.AddProduct(this.product2);
        var product3Id = this.AddProduct(this.product3);

        this.product1.Id = product1Id;
        this.product2.Id = product2Id;
        this.product3.Id = product3Id;
    }

    private int AddProduct(Product product)
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

            return (int) command.ExecuteScalar();
        }
    }


    private void DeployDatabase()
    {
        string dacpacPath = @"..\..\..\..\ADO\bin\Debug\ADO.dacpac";

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
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
    // no code needed here, xUnit just uses this as a marker
}