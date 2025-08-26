using ConsoleApp;
using System.Transactions;

namespace ADO.Tests
{
    [Collection("Database collection")]
    public class OrdersRepositoryTests
    {
        private readonly DatabaseFixture _fixture;

        public OrdersRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }


        [Fact]
        public void CreateOrder()
        {
            var ordersRepository = new OrdersRepository(this._fixture.ConnectionString);

            using (var scope = new TransactionScope())
            {
                this._fixture.AddProducts();

                var createParams = new CreateOrderParams()
                {
                    ProductId = _fixture.product1.Id,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    Status = OrderStatus.NotStarted,
                };

                var id = ordersRepository.CreateOrder(createParams);

                var order = ordersRepository.GetOrder(id);

                Assert.NotNull(order);
            }
        }

        [Fact]
        public void UpdateOrder()
        {
            var ordersRepository = new OrdersRepository(this._fixture.ConnectionString);

            using (var scope = new TransactionScope())
            {
                this._fixture.AddProducts();

                var createParams = new CreateOrderParams()
                {
                    ProductId = _fixture.product1.Id,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    Status = OrderStatus.NotStarted,
                };

                var id = ordersRepository.CreateOrder(createParams);

                var updateParams = new UpdateOrderParams()
                {
                    CreatedDate = DateTime.Parse("2025-08-26 14:45:30"),
                    UpdatedDate = DateTime.Parse("2025-08-28 14:45:30"),
                    ProductId = createParams.ProductId,
                    Status = OrderStatus.Arrived,
                };

                ordersRepository.UpdateOrder(id, updateParams);


                var expectedOrder = new Order() {
                    Id = id,
                    ProductId = createParams.ProductId,
                    CreatedDate = DateTime.Parse("2025-08-26 14:45:30"),
                    UpdatedDate = DateTime.Parse("2025-08-28 14:45:30"),
                    Status = OrderStatus.Arrived,
                };

                var updatedOrder = ordersRepository.GetOrder(id);

                Assert.Equal(expectedOrder, updatedOrder);
            }
        }

        [Fact]
        public void DeleteOrder()
        {
            var repository = new OrdersRepository(this._fixture.ConnectionString);

            using (var scope = new TransactionScope())
            {
                this._fixture.AddProducts();

                var createParams = new CreateOrderParams()
                {
                    ProductId = _fixture.product1.Id,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    Status = OrderStatus.NotStarted,
                };

                var id = repository.CreateOrder(createParams);

                repository.DeleteOrder(id);

                var orders = repository.ListOrders();

                Assert.DoesNotContain(orders, o => o.Id == id);
            }
        }

        [Fact]
        public void ListOrders()
        {
            var repository = new OrdersRepository(this._fixture.ConnectionString);

            using (var scope = new TransactionScope())
            {
                this._fixture.AddProducts();

                var createParams1 = new CreateOrderParams()
                {
                    ProductId = _fixture.product1.Id,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    Status = OrderStatus.NotStarted,
                };

                var id1 = repository.CreateOrder(createParams1);

                var createParams2 = new CreateOrderParams()
                {
                    ProductId = _fixture.product2.Id,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    Status = OrderStatus.Arrived,
                };

                var id2 = repository.CreateOrder(createParams2);

                var orders = repository.ListOrders();

                Assert.Contains(orders, (p) => p.Id == id1);
                Assert.Contains(orders, (p) => p.Id == id2);
            }
        }
    }
}