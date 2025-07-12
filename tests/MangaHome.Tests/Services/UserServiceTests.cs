using MangaHome.Api.Common;
using MangaHome.Api.Exceptions;
using MangaHome.Api.Models.Requests;
using MangaHome.Api.Services;
using MangaHome.Api.Services.Implementations;
using MangaHome.Core.Models;
using MangaHome.Core.Values;
using MangaHome.Infrastructure.Contexts;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;

namespace MangaHome.Tests.Services;

public class UserServiceTests
{
    private IUserService _userService;
    private readonly Mock<IDistributedCache> _mockDistributedCache;
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<MangaHomeDbContext> _options;

    public UserServiceTests()
    {
        _mockDistributedCache = new Mock<IDistributedCache>();

        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<MangaHomeDbContext>()
            .UseSqlite(_connection)
            .Options;

        using (var context = new MangaHomeDbContext(_options))
            context.Database.EnsureCreated();
    }

    [Fact]
    public async Task GetUserByIdAsync_WithExistingUserId_ShouldReturnUser()
    {
        //Arrange
        using var context = new MangaHomeDbContext(_options);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        var user = new User("test1", "test@mail.com", Role.User, "Test123@", []);
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        _userService = new UserService(context, _mockDistributedCache.Object);

        //Act
        var resultUser = await _userService.GetUserByIdAsync((Guid)user.Id!, It.IsAny<CancellationToken>());

        //Assert
        Assert.Equal(resultUser.Id, user.Id);
        Assert.Equal(resultUser.Username, user.Username);
        Assert.Equal(resultUser.Email, user.Email);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithNonExistingUserId_ShouldThrowException()
    {
        //Arrange
        using var context = new MangaHomeDbContext(_options);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        _userService = new UserService(context, _mockDistributedCache.Object);

        //Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _userService.GetUserByIdAsync(Guid.Empty, It.IsAny<CancellationToken>()));

        //Assert
        Assert.Equal(Messages.ERR_USER_NOT_FOUND, exception.Message);
    }

    [Theory]
    [InlineData("test", "test@mail.com", "Test123@")]
    [InlineData("jonedoe1234567891234", "john.doe@mail.com", "Test123456789123456789123456789@")]
    [InlineData("jane.doe", "jane.doe@mail.com", "Jane123Doe#")]
    public async Task RegisterUserAsync_WithValidInput_ShouldReturnUserId(string username, string email, string password)
    {
        //Arrange
        var request = new RegisterUserRequest
        {
            Username = username,
            Email = email,
            Password = password
        };

        using var context = new MangaHomeDbContext(_options);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        _userService = new UserService(context, _mockDistributedCache.Object);

        //Act
        var result = await _userService.RegisterUserAsync(request, It.IsAny<CancellationToken>());

        //Assert
        Assert.Equal(username, result.Username);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public async Task RegisterUserAsync_WithExistingEmail_ShouldThrowException()
    {
        //Arrange
        var existingEmail = "test@mail.com";

        var request = new RegisterUserRequest
        {
            Username = "test1",
            Email = existingEmail,
            Password = "Test123@"
        };

        using var context = new MangaHomeDbContext(_options);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        await context.Users.AddAsync(new User("test2", existingEmail, Role.User, "Test123@", []));
        await context.SaveChangesAsync();

        _userService = new UserService(context, _mockDistributedCache.Object);

        //Act
        var exception = await Assert.ThrowsAsync<BadRequestException>(async () =>
            await _userService.RegisterUserAsync(request, It.IsAny<CancellationToken>()));

        //Assert
        Assert.Equal(Messages.VAL_EMAIL_ALREADY_EXISTED, exception.Message);
    }

    [Fact]
    public async Task RegisterUserAsync_WithExistingUsername_ShouldThrowException()
    {
        //Arrange
        var existingUsername = "test";

        var request = new RegisterUserRequest
        {
            Username = existingUsername,
            Email = "test1@mail.com",
            Password = "Test123@"
        };

        using var context = new MangaHomeDbContext(_options);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        await context.Users.AddAsync(new User(existingUsername, "test2@mail.com", Role.User, "Test123@", []));
        await context.SaveChangesAsync();

        _userService = new UserService(context, _mockDistributedCache.Object);

        //Act
        var exception = await Assert.ThrowsAsync<BadRequestException>(async () =>
            await _userService.RegisterUserAsync(request, It.IsAny<CancellationToken>()));

        //Assert
        Assert.Equal(Messages.VAL_USERNAME_ALREADY_EXISTED, exception.Message);
    }
}