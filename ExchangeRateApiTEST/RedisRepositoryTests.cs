using ExchangeRateAPI.Repositories;
using System;
using System.Text.Json;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace ExchangeRateApiTests
{
    public class RedisRepositoryTests
    {
        private readonly Mock<IConnectionMultiplexer> _mockConnectionMultiplexer;
        private readonly Mock<IDatabase> _mockDatabase;
        private readonly RedisRepository _repository;

        public RedisRepositoryTests()
        {
            _mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
            _mockDatabase = new Mock<IDatabase>();

            _mockConnectionMultiplexer
                .Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object?>()))
                .Returns(_mockDatabase.Object);

            _repository = new RedisRepository(_mockConnectionMultiplexer.Object);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnDataFromRedis_WhenKeyExists()
        {
            var key = "test-key";
            var expectedValue = new TestModel(1, "Test");
            var serializedValue = JsonSerializer.Serialize(expectedValue);

            _mockDatabase
                .Setup(db => db.StringGetAsync((RedisKey)key, CommandFlags.None))
                .ReturnsAsync((RedisValue)serializedValue);

            var result = await _repository.GetAsync<TestModel>(key);

            Assert.NotNull(result);
            Assert.Equal(expectedValue.Id, result!.Id);
            Assert.Equal(expectedValue.Name, result.Name);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNull_WhenKeyDoesNotExist()
        {
            var key = "nonexistent-key";

            _mockDatabase
                .Setup(db => db.StringGetAsync((RedisKey)key, CommandFlags.None))
                .ReturnsAsync(RedisValue.Null);

            var result = await _repository.GetAsync<TestModel>(key);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_ShouldHandleDeserializationException()
        {
            var key = "test-key";
            var invalidJson = "{ invalid json }";

            _mockDatabase
                .Setup(db => db.StringGetAsync((RedisKey)key, CommandFlags.None))
                .ReturnsAsync((RedisValue)invalidJson);

            await Assert.ThrowsAsync<JsonException>(() => _repository.GetAsync<TestModel>(key));
        }
    }

    public class TestModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public TestModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}

   
       