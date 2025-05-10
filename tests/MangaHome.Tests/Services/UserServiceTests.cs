using AutoMapper;
using MangaHome.Api.Models.ViewModels;
using MangaHome.Api.Models.Requests;
using MangaHome.Api.Services;
using MangaHome.Api.Services.Implementations;
using MangaHome.Core.Abstractions;
using MangaHome.Infrastructure.Contexts;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MangaHome.Tests.Services;

public class UserServiceTests
{
    private readonly IUserService _userService;
    private readonly Mock<ILogger<MangaHomeDbContext>> _mockLogger;
    private readonly Mock<IDateTimeService> _mockDateTimeService;
    private readonly Mock<IRequestInfoService> _mockHttpContextInfoService;
    private readonly Mock<IDistributedCache> _mockDistributedCache;
    private readonly Mock<IMapper> _mockMapper;
    private readonly MangaHomeDbContext _dbContext;
    private readonly Mock<IPasswordHashingService> _mockPasswordHashingService;

    public UserServiceTests()
    {
        _mockLogger = new Mock<ILogger<MangaHomeDbContext>>();
        _mockDateTimeService = new Mock<IDateTimeService>();
        _mockHttpContextInfoService = new Mock<IRequestInfoService>();

        _mockDistributedCache = new Mock<IDistributedCache>();
        _mockMapper = new Mock<IMapper>();
        _dbContext = CreateSqlLiteDbContext();
        _mockPasswordHashingService = new Mock<IPasswordHashingService>();

        _userService = new UserService(_mockDistributedCache.Object,
            _mockMapper.Object,
            _dbContext,
            _mockPasswordHashingService.Object);
    }

    private MangaHomeDbContext CreateSqlLiteDbContext()
    {
        var _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var _contextOptions = new DbContextOptionsBuilder<MangaHomeDbContext>()
            .UseSqlite(_connection)
            .Options;

        return new MangaHomeDbContext(_contextOptions,
            _mockDateTimeService.Object,
            _mockHttpContextInfoService.Object,
            _mockLogger.Object);
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
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.IsType<UserViewModel>(result.Value);
            Assert.Equal(username, result.Value.Username);
            Assert.Equal(email, result.Value.Email);
        }
    }
}