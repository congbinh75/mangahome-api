using MangaHome.Api.Models.Requests;
using MangaHome.Api.Services;
using MangaHome.Api.Services.Implementations;
using MangaHome.Infrastructure.Contexts;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;

namespace MangaHome.Tests.Services;

public class UserServiceTests
{
    private readonly UserService _userService;
    private readonly Mock<IDistributedCache> _mockDistributedCache;
    private readonly MangaHomeDbContext _dbContext;
    private readonly Mock<IPasswordHashingService> _mockPasswordHashingService;

    public UserServiceTests()
    {
        _mockDistributedCache = new Mock<IDistributedCache>();
        _dbContext = CreateSqlLiteDbContext();
        _mockPasswordHashingService = new Mock<IPasswordHashingService>();

        _userService = new UserService(_dbContext,
            _mockDistributedCache.Object,
            _mockPasswordHashingService.Object);
    }

    private static MangaHomeDbContext CreateSqlLiteDbContext()
    {
        var _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var _contextOptions = new DbContextOptionsBuilder<MangaHomeDbContext>()
            .UseSqlite(_connection)
            .Options;

        return new MangaHomeDbContext(_contextOptions);
    }

    [Theory]
    [InlineData("test", "test@mail.com", "Test123@")]
    [InlineData("jonedoe1234567891234", "john.doe@mail.com", "Test123456789123456789123456789@")]
    [InlineData("jane.doe", "jane.doe@mail.com", "Jane123Doe#")]
    public async Task RegisterUserAsync_WithValidInput_ShouldReturnUserDto(string username, string email, string password)
    {
        var request = new RegisterUserRequest
        {
            Username = username,
            Email = email,
            Password = password
        };

        _mockPasswordHashingService.Setup(x => x.HashPassword(password, It.IsAny<byte[]>())).Returns(("", Array.Empty<byte>()));

        if (await _dbContext.Database.EnsureCreatedAsync())
        {
            var result = await _userService.RegisterUserAsync(request, It.IsAny<CancellationToken>());
            Assert.Equal(username, result.Username);
            Assert.Equal(email, result.Email);
        }
    }
}