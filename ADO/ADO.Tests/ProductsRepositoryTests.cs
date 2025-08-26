using ConsoleApp;
using System.Transactions;

namespace ADO.Tests
{
    public class ProductsRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public ProductsRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }


        [Fact]
        public void CreateProduct()
        {
            var productsRepository = new ProductsRepository(this._fixture.ConnectionString);

            using (var scope = new TransactionScope())
            {
                var createParams = GetCreateProductParams();

                var id = productsRepository.CreateProduct(createParams);

                var product = productsRepository.GetProduct(id);

                Assert.NotNull(product);
                Assert.Equal(createParams.Name, product?.Name);
            }
        }

        [Fact]
        public void UpdateProduct()
        {
            var productsRepository = new ProductsRepository(this._fixture.ConnectionString);

            using (var scope = new TransactionScope())
            {
                var createParams = GetCreateProductParams();

                var id = productsRepository.CreateProduct(createParams);
                var updateParams = new UpdateProductParams()
                {
                    Name = "x",
                    Description = "x",
                    Height = 1m,
                    Weight = 2m,
                    Length = 3m,
                    Width = 4m,
                };

                productsRepository.UpdateProduct(id, updateParams);

                var expectedUpdatedProduct = new Product()
                {
                    Id = id,
                    Name = updateParams.Name,
                    Description = updateParams.Description,
                    Height = updateParams.Height,
                    Weight = updateParams.Weight,
                    Length = updateParams.Length,
                    Width = updateParams.Width,
                };

                var product = productsRepository.GetProduct(id);

                Assert.Equal(expectedUpdatedProduct, product);
            }
        }

        [Fact]
        public void DeleteProduct()
        {
            var productsRepository = new ProductsRepository(this._fixture.ConnectionString);

            using (var scope = new TransactionScope())
            {
                var createParams = GetCreateProductParams();

                var id = productsRepository.CreateProduct(createParams);

                productsRepository.DeleteProduct(id);

                var products = productsRepository.ListProducts();

                Assert.DoesNotContain(products, p => p.Id == id);
            }
        }

        [Fact]
        public void ListProducts()
        {
            var productsRepository = new ProductsRepository(this._fixture.ConnectionString);

            using (var scope = new TransactionScope())
            {
                var createParams1 = GetCreateProductParams();
                var createParams2 = GetCreateProductParams();

                var id1 = productsRepository.CreateProduct(createParams1);
                var id2 = productsRepository.CreateProduct(createParams2);

                var products = productsRepository.ListProducts();

                Assert.Contains(products, (p) => p.Id == id1);
                Assert.Contains(products, (p) => p.Id == id2);
            }
        }

        private CreateProductParams GetCreateProductParams()
        {
            var createParams = new CreateProductParams
            {
                Name = "iPhone 15",
                Description = "Latest Apple smartphone",
                Weight = 0.174m,
                Height = 14.7m,
                Length = 7.1m,
                Width = 0.75m
            };

            return createParams;
        }
    }
}