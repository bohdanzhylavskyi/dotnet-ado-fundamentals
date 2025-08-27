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

        [Fact]
        public void SearchOrdersByMonth()
        {
            var repository = new OrdersRepository(this._fixture.ConnectionString);

            using (var scope = new TransactionScope())
            {
                this._fixture.AddProducts();

                var createParams1 = new CreateOrderParams()
                {
                    ProductId = _fixture.product1.Id,
                    CreatedDate = DateTime.Parse("01/26/2025 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("01/26/2025 2:30:00 PM"),
                    Status = OrderStatus.NotStarted,
                };

                var id1 = repository.CreateOrder(createParams1);

                var createParams2 = new CreateOrderParams()
                {
                    ProductId = _fixture.product2.Id,
                    CreatedDate = DateTime.Parse("08/26/2025 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("08/26/2025 2:30:00 PM"),
                    Status = OrderStatus.Arrived,
                };

                var id2 = repository.CreateOrder(createParams2);

                var orders = repository.SearchOrdersByMonth(8);

                Assert.Equal(1, orders.Count);
                Assert.Contains(orders, (p) => p.Id == id2);
            }
        }

        [Fact]
        public void SearchOrdersByYear()
        {
            var repository = new OrdersRepository(this._fixture.ConnectionString);

            using (var scope = new TransactionScope())
            {
                this._fixture.AddProducts();

                var createParams1 = new CreateOrderParams()
                {
                    ProductId = _fixture.product1.Id,
                    CreatedDate = DateTime.Parse("01/26/2022 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("01/26/2022 2:30:00 PM"),
                    Status = OrderStatus.NotStarted,
                };

                var id1 = repository.CreateOrder(createParams1);

                var createParams2 = new CreateOrderParams()
                {
                    ProductId = _fixture.product2.Id,
                    CreatedDate = DateTime.Parse("08/26/2025 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("08/26/2025 2:30:00 PM"),
                    Status = OrderStatus.Arrived,
                };

                var id2 = repository.CreateOrder(createParams2);

                var orders = repository.SearchOrdersByYear(2025);


                Assert.Equal(1, orders.Count);
                Assert.Contains(orders, (p) => p.Id == id2);
            }
        }

        [Fact]
        public void SearchOrdersByStatus()
        {
            var repository = new OrdersRepository(this._fixture.ConnectionString);

            using (var scope = new TransactionScope())
            {
                this._fixture.AddProducts();

                var createParams1 = new CreateOrderParams()
                {
                    ProductId = _fixture.product1.Id,
                    CreatedDate = DateTime.Parse("01/26/2022 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("01/26/2022 2:30:00 PM"),
                    Status = OrderStatus.NotStarted,
                };

                var id1 = repository.CreateOrder(createParams1);

                var createParams2 = new CreateOrderParams()
                {
                    ProductId = _fixture.product2.Id,
                    CreatedDate = DateTime.Parse("08/26/2025 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("08/26/2025 2:30:00 PM"),
                    Status = OrderStatus.Arrived,
                };

                var id2 = repository.CreateOrder(createParams2);

                var orders = repository.SearchOrdersByStatus(OrderStatus.Arrived);


                Assert.Equal(1, orders.Count);
                Assert.Contains(orders, (p) => p.Id == id2);
            }
        }

        [Fact]
        public void SearchOrdersByProduct()
        {
            var repository = new OrdersRepository(this._fixture.ConnectionString);

            using (var scope = new TransactionScope())
            {
                this._fixture.AddProducts();

                var createParams1 = new CreateOrderParams()
                {
                    ProductId = _fixture.product1.Id,
                    CreatedDate = DateTime.Parse("01/26/2022 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("01/26/2022 2:30:00 PM"),
                    Status = OrderStatus.NotStarted,
                };

                var id1 = repository.CreateOrder(createParams1);

                var createParams2 = new CreateOrderParams()
                {
                    ProductId = _fixture.product2.Id,
                    CreatedDate = DateTime.Parse("08/26/2025 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("08/26/2025 2:30:00 PM"),
                    Status = OrderStatus.Arrived,
                };

                var id2 = repository.CreateOrder(createParams2);

                var orders = repository.SearchOrdersByProduct(_fixture.product2.Id);

                Assert.Equal(1, orders.Count);
                Assert.Contains(orders, (p) => p.Id == id2);
            }
        }

        [Fact]
        public void DeleteOrdersByMonth()
        {
            var repository = new OrdersRepository(this._fixture.ConnectionString);

            using (var scope = new TransactionScope())
            {
                this._fixture.AddProducts();

                var createParams1 = new CreateOrderParams()
                {
                    ProductId = _fixture.product1.Id,
                    CreatedDate = DateTime.Parse("01/26/2025 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("01/26/2025 2:30:00 PM"),
                    Status = OrderStatus.NotStarted,
                };

                var id1 = repository.CreateOrder(createParams1);

                var createParams2 = new CreateOrderParams()
                {
                    ProductId = _fixture.product2.Id,
                    CreatedDate = DateTime.Parse("08/26/2025 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("08/26/2025 2:30:00 PM"),
                    Status = OrderStatus.Arrived,
                };

                var id2 = repository.CreateOrder(createParams2);

                repository.DeleteOrdersByMonth(1);

                var orders = repository.ListOrders();

                Assert.Equal(1, orders.Count);
                Assert.Contains(orders, (p) => p.Id == id2);
            }
        }

        [Fact]
        public void DeleteOrdersByYear()
        {
            var repository = new OrdersRepository(this._fixture.ConnectionString);

            using (var scope = new TransactionScope())
            {
                this._fixture.AddProducts();

                var createParams1 = new CreateOrderParams()
                {
                    ProductId = _fixture.product1.Id,
                    CreatedDate = DateTime.Parse("01/26/2020 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("01/26/2020 2:30:00 PM"),
                    Status = OrderStatus.NotStarted,
                };

                var id1 = repository.CreateOrder(createParams1);

                var createParams2 = new CreateOrderParams()
                {
                    ProductId = _fixture.product2.Id,
                    CreatedDate = DateTime.Parse("08/26/2026 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("08/26/2026 2:30:00 PM"),
                    Status = OrderStatus.Arrived,
                };

                var id2 = repository.CreateOrder(createParams2);

                repository.DeleteOrdersByYear(2020);

                var orders = repository.ListOrders();

                Assert.Equal(1, orders.Count);
                Assert.Contains(orders, (p) => p.Id == id2);
            }
        }

        [Fact]
        public void DeleteOrdersByStatus()
        {
            var repository = new OrdersRepository(this._fixture.ConnectionString);

            using (var scope = new TransactionScope())
            {
                this._fixture.AddProducts();

                var createParams1 = new CreateOrderParams()
                {
                    ProductId = _fixture.product1.Id,
                    CreatedDate = DateTime.Parse("01/26/2020 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("01/26/2020 2:30:00 PM"),
                    Status = OrderStatus.NotStarted,
                };

                var id1 = repository.CreateOrder(createParams1);

                var createParams2 = new CreateOrderParams()
                {
                    ProductId = _fixture.product2.Id,
                    CreatedDate = DateTime.Parse("08/26/2026 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("08/26/2026 2:30:00 PM"),
                    Status = OrderStatus.Arrived,
                };

                var id2 = repository.CreateOrder(createParams2);

                repository.DeleteOrdersByStatus(OrderStatus.NotStarted);

                var orders = repository.ListOrders();

                Assert.Equal(1, orders.Count);
                Assert.Contains(orders, (p) => p.Id == id2);
            }
        }

        [Fact]
        public void DeleteOrdersByProduct()
        {
            var repository = new OrdersRepository(this._fixture.ConnectionString);

            using (var scope = new TransactionScope())
            {
                this._fixture.AddProducts();

                var createParams1 = new CreateOrderParams()
                {
                    ProductId = _fixture.product1.Id,
                    CreatedDate = DateTime.Parse("01/26/2020 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("01/26/2020 2:30:00 PM"),
                    Status = OrderStatus.NotStarted,
                };

                var id1 = repository.CreateOrder(createParams1);

                var createParams2 = new CreateOrderParams()
                {
                    ProductId = _fixture.product2.Id,
                    CreatedDate = DateTime.Parse("08/26/2026 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("08/26/2026 2:30:00 PM"),
                    Status = OrderStatus.Arrived,
                };

                var id2 = repository.CreateOrder(createParams2);

                repository.DeleteOrdersByProduct(_fixture.product1.Id);

                var orders = repository.ListOrders();

                Assert.Equal(1, orders.Count);
                Assert.Contains(orders, (p) => p.Id == id2);
            }
        }
    }
}