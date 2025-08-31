using ADO.Lib;
using System.Transactions;

namespace ADO.Tests
{
    [Collection("Database collection")]
    public class OrdersRepositoryTests
    {
        private readonly DatabaseFixture _fixture;
        private readonly OrdersRepository _repository;

        public OrdersRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new OrdersRepository(this._fixture.ConnectionString);
        }

        [Fact]
        public void CreateOrder()
        {
            using (var scope = new TransactionScope())
            {
                this._fixture.SeedProducts();

                var order = new Order()
                {
                    ProductId = _fixture.product1.Id,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    Status = OrderStatus.NotStarted,
                };

                // act
                _repository.CreateOrder(order);

                // assert
                var createdOrder = _repository.GetOrder(order.Id);

                Assert.NotNull(createdOrder);
            }
        }

        [Fact]
        public void GetOrder()
        {
            using (var scope = new TransactionScope())
            {
                this._fixture.SeedProducts();

                var order = new Order()
                {
                    ProductId = _fixture.product1.Id,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    Status = OrderStatus.NotStarted,
                };

                _repository.CreateOrder(order);

                // act
                var createdOrder = _repository.GetOrder(order.Id);

                Assert.NotNull(createdOrder);
            }
        }

        [Fact]
        public void UpdateOrder()
        {
            using (var scope = new TransactionScope())
            {
                this._fixture.SeedProducts();

                var order = new Order()
                {
                    ProductId = _fixture.product1.Id,
                    CreatedDate = DateTime.Parse("2022-01-21 00:00:00"),
                    UpdatedDate = DateTime.Parse("2022-05-10 00:00:00"),
                    Status = OrderStatus.NotStarted,
                };

                _repository.CreateOrder(order);

                // act
                order.CreatedDate = DateTime.Parse("2025-08-26 14:45:30");
                order.UpdatedDate = DateTime.Parse("2025-08-28 14:45:30");
                order.Status = OrderStatus.Arrived;

                _repository.UpdateOrder(order);

                // assert
                var retrievedOrder = _repository.GetOrder(order.Id);

                Assert.Equivalent(order, retrievedOrder);
            }
        }

        [Fact]
        public void DeleteOrder()
        {
            using (var scope = new TransactionScope())
            {
                this._fixture.SeedProducts();

                var order = new Order()
                {
                    ProductId = _fixture.product1.Id,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    Status = OrderStatus.NotStarted,
                };

                _repository.CreateOrder(order);

                // act
                _repository.DeleteOrder(order.Id);

                var orders = _repository.ListOrders();

                Assert.DoesNotContain(orders, o => o.Id == order.Id);
            }
        }

        [Fact]
        public void ListOrders()
        {
            using (var scope = new TransactionScope())
            {
                this._fixture.SeedProducts();

                var order1 = new Order()
                {
                    ProductId = _fixture.product1.Id,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    Status = OrderStatus.NotStarted,
                };

                _repository.CreateOrder(order1);

                var order2 = new Order()
                {
                    ProductId = _fixture.product2.Id,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    Status = OrderStatus.Arrived,
                };

                _repository.CreateOrder(order2);

                // act
                var orders = _repository.ListOrders();

                Assert.Contains(orders, (o) => o.Id == order1.Id);
                Assert.Contains(orders, (o) => o.Id == order2.Id);
            }
        }

        [Fact]
        public void SearchOrdersByMonth()
        {
            using (var scope = new TransactionScope())
            {
                this._fixture.SeedProducts();

                var order1 = new Order()
                {
                    ProductId = _fixture.product1.Id,
                    CreatedDate = DateTime.Parse("01/26/2025 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("01/26/2025 2:30:00 PM"),
                    Status = OrderStatus.NotStarted,
                };

                _repository.CreateOrder(order1);

                var order2 = new Order()
                {
                    ProductId = _fixture.product2.Id,
                    CreatedDate = DateTime.Parse("08/26/2025 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("08/26/2025 2:30:00 PM"),
                    Status = OrderStatus.Arrived,
                };

                _repository.CreateOrder(order2);

                // act
                var orders = _repository.SearchOrdersByMonth(8);

                Assert.Equal(1, orders.Count);
                Assert.Contains(orders, (o) => o.Id == order2.Id);
            }
        }

        [Fact]
        public void SearchOrdersByYear()
        {
            using (var scope = new TransactionScope())
            {
                this._fixture.SeedProducts();

                var order1 = new Order()
                {
                    ProductId = _fixture.product1.Id,
                    CreatedDate = DateTime.Parse("01/26/2022 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("01/26/2022 2:30:00 PM"),
                    Status = OrderStatus.NotStarted,
                };

                _repository.CreateOrder(order1);

                var order2 = new Order()
                {
                    ProductId = _fixture.product2.Id,
                    CreatedDate = DateTime.Parse("08/26/2025 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("08/26/2025 2:30:00 PM"),
                    Status = OrderStatus.Arrived,
                };

                _repository.CreateOrder(order2);

                // act
                var orders = _repository.SearchOrdersByYear(2025);

                Assert.Equal(1, orders.Count);
                Assert.Contains(orders, (o) => o.Id == order2.Id);
            }
        }

        [Fact]
        public void SearchOrdersByStatus()
        {
            using (var scope = new TransactionScope())
            {
                this._fixture.SeedProducts();

                var order1 = new Order()
                {
                    ProductId = _fixture.product1.Id,
                    CreatedDate = DateTime.Parse("01/26/2022 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("01/26/2022 2:30:00 PM"),
                    Status = OrderStatus.NotStarted,
                };

                _repository.CreateOrder(order1);

                var order2 = new Order()
                {
                    ProductId = _fixture.product2.Id,
                    CreatedDate = DateTime.Parse("08/26/2025 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("08/26/2025 2:30:00 PM"),
                    Status = OrderStatus.Arrived,
                };

                _repository.CreateOrder(order2);

                // act
                var orders = _repository.SearchOrdersByStatus(OrderStatus.Arrived);

                Assert.Equal(1, orders.Count);
                Assert.Contains(orders, (o) => o.Id == order2.Id);
            }
        }

        [Fact]
        public void SearchOrdersByProduct()
        {
            using (var scope = new TransactionScope())
            {
                this._fixture.SeedProducts();

                var order1 = new Order()
                {
                    ProductId = _fixture.product1.Id,
                    CreatedDate = DateTime.Parse("01/26/2022 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("01/26/2022 2:30:00 PM"),
                    Status = OrderStatus.NotStarted,
                };

                _repository.CreateOrder(order1);

                var order2 = new Order()
                {
                    ProductId = _fixture.product2.Id,
                    CreatedDate = DateTime.Parse("08/26/2025 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("08/26/2025 2:30:00 PM"),
                    Status = OrderStatus.Arrived,
                };

                _repository.CreateOrder(order2);

                // act
                var orders = _repository.SearchOrdersByProduct(_fixture.product2.Id);

                Assert.Equal(1, orders.Count);
                Assert.Contains(orders, (o) => o.Id == order2.Id);
            }
        }

        [Fact]
        public void DeleteOrdersByMonth()
        {
            using (var scope = new TransactionScope())
            {
                this._fixture.SeedProducts();

                var order1 = new Order()
                {
                    ProductId = _fixture.product1.Id,
                    CreatedDate = DateTime.Parse("01/26/2025 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("01/26/2025 2:30:00 PM"),
                    Status = OrderStatus.NotStarted,
                };

                _repository.CreateOrder(order1);

                var order2 = new Order()
                {
                    ProductId = _fixture.product2.Id,
                    CreatedDate = DateTime.Parse("08/26/2025 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("08/26/2025 2:30:00 PM"),
                    Status = OrderStatus.Arrived,
                };

                _repository.CreateOrder(order2);

                // act
                _repository.DeleteOrdersByMonth(1);

                // assert
                var orders = _repository.ListOrders();

                Assert.Equal(1, orders.Count);
                Assert.Contains(orders, (o) => o.Id == order2.Id);
            }
        }

        [Fact]
        public void DeleteOrdersByYear()
        {
            using (var scope = new TransactionScope())
            {
                this._fixture.SeedProducts();

                var order1 = new Order()
                {
                    ProductId = _fixture.product1.Id,
                    CreatedDate = DateTime.Parse("01/26/2020 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("01/26/2020 2:30:00 PM"),
                    Status = OrderStatus.NotStarted,
                };

                _repository.CreateOrder(order1);

                var order2 = new Order()
                {
                    ProductId = _fixture.product2.Id,
                    CreatedDate = DateTime.Parse("08/26/2026 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("08/26/2026 2:30:00 PM"),
                    Status = OrderStatus.Arrived,
                };

                _repository.CreateOrder(order2);

                // act
                _repository.DeleteOrdersByYear(2020);

                // assert
                var orders = _repository.ListOrders();

                Assert.Equal(1, orders.Count);
                Assert.Contains(orders, (o) => o.Id == order2.Id);
            }
        }

        [Fact]
        public void DeleteOrdersByStatus()
        {
            using (var scope = new TransactionScope())
            {
                this._fixture.SeedProducts();

                var order1 = new Order()
                {
                    ProductId = _fixture.product1.Id,
                    CreatedDate = DateTime.Parse("01/26/2020 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("01/26/2020 2:30:00 PM"),
                    Status = OrderStatus.NotStarted,
                };

                _repository.CreateOrder(order1);

                var order2 = new Order()
                {
                    ProductId = _fixture.product2.Id,
                    CreatedDate = DateTime.Parse("08/26/2026 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("08/26/2026 2:30:00 PM"),
                    Status = OrderStatus.Arrived,
                };

                _repository.CreateOrder(order2);

                // act
                _repository.DeleteOrdersByStatus(OrderStatus.NotStarted);

                // assert
                var orders = _repository.ListOrders();

                Assert.Equal(1, orders.Count);
                Assert.Contains(orders, (o) => o.Id == order2.Id);
            }
        }

        [Fact]
        public void DeleteOrdersByProduct()
        {
            using (var scope = new TransactionScope())
            {
                this._fixture.SeedProducts();

                var order1 = new Order()
                {
                    ProductId = _fixture.product1.Id,
                    CreatedDate = DateTime.Parse("01/26/2020 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("01/26/2020 2:30:00 PM"),
                    Status = OrderStatus.NotStarted,
                };

                _repository.CreateOrder(order1);

                var order2 = new Order()
                {
                    ProductId = _fixture.product2.Id,
                    CreatedDate = DateTime.Parse("08/26/2026 2:30:00 PM"),
                    UpdatedDate = DateTime.Parse("08/26/2026 2:30:00 PM"),
                    Status = OrderStatus.Arrived,
                };

                _repository.CreateOrder(order2);

                // act
                _repository.DeleteOrdersByProduct(_fixture.product1.Id);

                // assert
                var orders = _repository.ListOrders();

                Assert.Equal(1, orders.Count);
                Assert.Contains(orders, (o) => o.Id == order2.Id);
            }
        }
    }
}