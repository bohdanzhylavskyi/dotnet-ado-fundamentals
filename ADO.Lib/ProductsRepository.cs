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
        private readonly SqlDataAdapter _adapter;
        private readonly DataTable _productsTable;
        private bool _isInitialized = false;

        public ProductsRepository(string connectionString)
        {
            this._connectionString = connectionString;
            this._adapter = new SqlDataAdapter("SELECT * FROM Products", _connectionString);
            this._productsTable = new DataTable();

            var insertCommand = new SqlCommand(
                "INSERT INTO Products (Name, Description, Weight, Height, Width, Length) " +
                "OUTPUT INSERTED.Id " +
                "VALUES (@Name, @Description, @Weight, @Height, @Width, @Length);",
                new SqlConnection(_connectionString));

            insertCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 0, "Name");
            insertCommand.Parameters.Add("@Description", SqlDbType.NVarChar, 0, "Description");
            insertCommand.Parameters.Add("@Weight", SqlDbType.Decimal, 0, "Weight");
            insertCommand.Parameters.Add("@Height", SqlDbType.Decimal, 0, "Height");
            insertCommand.Parameters.Add("@Width", SqlDbType.Decimal, 0, "Width");
            insertCommand.Parameters.Add("@Length", SqlDbType.Decimal, 0, "Length");

            this._adapter.InsertCommand = insertCommand;
        }

        public Product? GetProduct(int productId)
        {
            this.Init();

            var row = this._productsTable.AsEnumerable().First(r => r.Field<int>("Id") == productId);

            if (row == null)
            {
                return null;
            }

            return new Product()
            {
                Id = row.Field<int>("Id"),
                Name = row.Field<string>("Name"),
                Description = row.Field<string>("Description"),
                Height = row.Field<decimal>("Height"),
                Weight = row.Field<decimal>("Weight"),
                Width = row.Field<decimal>("Width"),
                Length = row.Field<decimal>("Length"),
            };
        }

        public int CreateProduct(CreateProductParams parameters)
        {
            this.Init();

            var row = this._productsTable.NewRow();

            row["Name"] = parameters.Name;
            row["Description"] = parameters.Description;
            row["Weight"] = parameters.Weight;
            row["Height"] = parameters.Height;
            row["Width"] = parameters.Width;
            row["Length"] = parameters.Length;

            this._productsTable.Rows.Add(row);

            this.Save();

            return (int)row["Id"]; 
        }

        public void UpdateProduct(int productId, UpdateProductParams parameters)
        {
            this.Init();

            var row = this._productsTable.AsEnumerable().First(r => r.Field<int>("Id") == productId);

            if (row != null)
            {
                row["Name"] = parameters.Name;
                row["Description"] = parameters.Description;
                row["Weight"] = parameters.Weight;
                row["Height"] = parameters.Height;
                row["Width"] = parameters.Width;
                row["Length"] = parameters.Length;
            }
        }

        public void DeleteProduct(int productId)
        {
            this.Init();

            var row = this._productsTable.AsEnumerable().First(r => r.Field<int>("Id") == productId);
            
            if (row != null)
            {
                row.Delete();
            }
        }

        public List<Product> ListProducts()
        {
            this.Init();

            var products = this._productsTable.AsEnumerable().Select(r => new Product
            {
                Id = r.Field<int>("Id"),
                Name = r.Field<string>("Name"),
                Description = r.Field<string>("Description"),
                Height = r.Field<decimal>("Height"),
                Weight = r.Field<decimal>("Weight"),
                Width = r.Field<decimal>("Width"),
                Length = r.Field<decimal>("Length"),
            }).ToList();

            return products;
        }

        public void Save()
        {
            this._adapter.Update(this._productsTable);
            this._productsTable.AcceptChanges();
        }

        private void Init()
        {
            if (!this._isInitialized)
            {
                var builder = new SqlCommandBuilder(this._adapter);
                this._adapter.Fill(this._productsTable);

                this._isInitialized = true;
            }
        }
    }
}
