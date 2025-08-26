using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Dac;

public class DatabaseFixture : IDisposable
{
    public string ConnectionString { get; }

    private string ServerConnectionString = "Server=EPUALVIW077B\\SQLSERVER2022;Integrated Security=true;TrustServerCertificate=True;";

    private const string TestDbName = "test-ado-fundamentals";

    public DatabaseFixture()
    {
        ConnectionString = $"{ServerConnectionString}Database={TestDbName};";

        DeployDatabase();
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
