using AutoMapper;
using FluentValidation.Results;
using MangaHome.Api.Common;
using MangaHome.Api.Controllers;
using MangaHome.Api.Models.Requests;
using MangaHome.Api.Models.Responses;
using MangaHome.Api.Models.ViewModels;
using MangaHome.Api.Services;
using MangaHome.Core.Models;
using MangaHome.Core.Values;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace MangaHome.Tests.Controllers;

public class UserControllerTests
{
    private readonly UserController _userController;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUserService> _mockUserService;

    public UserControllerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockUserService = new Mock<IUserService>();

        _userController = new UserController(_mockMapper.Object, _mockUserService.Object);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithExistingUserId_ShouldReturnUser()
    {
        //Arrange
        var user = new User("test1", "test@mail.com", Role.User, "Test123@", []);
        var userViewModel = new UserViewModel
        {
            Id = user.Id.ToString(),
            Username = user.Username,
            Email = user.Email,
            Role = user.Role.ToString(),
        };

        _mockUserService.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockMapper.Setup(x => x.Map<UserViewModel>(user)).Returns(userViewModel);

        //Act
        var result = await _userController.GetUserByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>());

        //Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);

        var okResult = result as OkObjectResult;
        Assert.IsType<Response<UserViewModel>>(okResult!.Value);

        var response = okResult.Value as Response<UserViewModel>;
        Assert.Equal(userViewModel.Id, response!.Data!.Id);
        Assert.Equal(userViewModel.Username, response.Data.Username);
        Assert.Equal(userViewModel.Email, response.Data.Email);
    }

    [Theory]
    [InlineData("test", "test@mail.com", "Test123@")]
    [InlineData("jonedoe1234567891234567891234567", "john.doe@mail.com", "Test123456789123456789123456789@")]
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
        var user = new User(request.Username, request.Email, Role.User, request.Password, []);

        _mockUserService.Setup(x => x.RegisterUserAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        //Act
        var result = await _userController.RegisterUserAsync(request, It.IsAny<CancellationToken>());

        //Assert
        Assert.NotNull(result);
        Assert.IsType<ObjectResult>(result);

        var createdResult = result as ObjectResult;
        Assert.Equal(201, createdResult!.StatusCode);
        Assert.IsType<Response<string>>(createdResult!.Value);

        var response = createdResult.Value as Response<string>;
        Assert.Equal(user.Id.ToString(), response!.Data);
    }

    [Theory]
    [InlineData("1234", "test@mail.com", "Test123@")]
    [InlineData("test@", "test@mail.com", "Test123@")]
    [InlineData("test#", "test@mail.com", "Test123@")]
    [InlineData("test+", "test@mail.com", "Test123@")]
    [InlineData("test-", "test@mail.com", "Test123@")]
    [InlineData("test/", "test@mail.com", "Test123@")]
    public async Task RegisterUserAsync_WithUsernameWithInvalidFormat_ShouldReturnErrors(string username, string email, string password)
    {
        //Arrange
        var request = new RegisterUserRequest
        {
            Username = username,
            Email = email,
            Password = password
        };
        var user = new User(request.Username, request.Email, Role.User, request.Password, []);

        var error = new Error
        {
            Message = Messages.VAL_USERNAME_FORMAT_NOT_VALID
        };

        _mockUserService.Setup(x => x.RegisterUserAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockMapper.Setup(x => x.Map<List<Error>>(It.IsAny<List<ValidationFailure>>())).Returns([error]);

        //Act
        var result = await _userController.RegisterUserAsync(request, It.IsAny<CancellationToken>());

        //Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);

        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsType<Response<UserViewModel>>(badRequestResult!.Value);

        var response = badRequestResult.Value as Response<UserViewModel>;
        Assert.Equal(Messages.VAL_USERNAME_FORMAT_NOT_VALID, response!.Errors!.First().Message);
    }

    [Theory]
    [InlineData("Test12345678901234567890123456789", "test@mail.com", "Test123@")]
    [InlineData("Tes", "test@mail.com", "Test123@")]
    public async Task RegisterUserAsync_WithUsernameWithInvalidLength_ShouldReturnErrors(string username, string email, string password)
    {
        //Arrange
        var request = new RegisterUserRequest
        {
            Username = username,
            Email = email,
            Password = password
        };
        var user = new User(request.Username, request.Email, Role.User, request.Password, []);

        var error = new Error
        {
            Message = Messages.VAL_USERNAME_LENGTH_NOT_VALID
        };

        _mockUserService.Setup(x => x.RegisterUserAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockMapper.Setup(x => x.Map<List<Error>>(It.IsAny<List<ValidationFailure>>())).Returns([error]);

        //Act
        var result = await _userController.RegisterUserAsync(request, It.IsAny<CancellationToken>());

        //Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);

        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsType<Response<UserViewModel>>(badRequestResult!.Value);

        var response = badRequestResult.Value as Response<UserViewModel>;
        Assert.Equal(Messages.VAL_USERNAME_LENGTH_NOT_VALID, response!.Errors!.First().Message);
    }

    [Theory]
    [InlineData("test1", "testmail.com", "Test123@")]
    [InlineData("test1", "testcom", "Test123@")]
    public async Task RegisterUserAsync_WithInvalidEmail_ShouldReturnErrors(string username, string email, string password)
    {
        //Arrange
        var request = new RegisterUserRequest
        {
            Username = username,
            Email = email,
            Password = password
        };
        var user = new User(request.Username, request.Email, Role.User, request.Password, []);

        var error = new Error
        {
            Message = Messages.VAL_EMAIL_NOT_VALID
        };

        _mockUserService.Setup(x => x.RegisterUserAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockMapper.Setup(x => x.Map<List<Error>>(It.IsAny<List<ValidationFailure>>())).Returns([error]);

        //Act
        var result = await _userController.RegisterUserAsync(request, It.IsAny<CancellationToken>());

        //Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);

        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsType<Response<UserViewModel>>(badRequestResult!.Value);

        var response = badRequestResult.Value as Response<UserViewModel>;
        Assert.Equal(Messages.VAL_EMAIL_NOT_VALID, response!.Errors!.First().Message);
    }

    [Theory]
    [InlineData("test1", "test@mail.com", "Test23@")]
    [InlineData("test1", "test@mail.com", "Test12345678901234567890123456789@")]
    public async Task RegisterUserAsync_WithPasswordWithInvalidLength_ShouldReturnErrors(string username, string email, string password)
    {
        //Arrange
        var request = new RegisterUserRequest
        {
            Username = username,
            Email = email,
            Password = password
        };
        var user = new User(request.Username, request.Email, Role.User, request.Password, []);

        var error = new Error
        {
            Message = Messages.VAL_PASSWORD_LENGTH_NOT_VALID
        };

        _mockUserService.Setup(x => x.RegisterUserAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockMapper.Setup(x => x.Map<List<Error>>(It.IsAny<List<ValidationFailure>>())).Returns([error]);

        //Act
        var result = await _userController.RegisterUserAsync(request, It.IsAny<CancellationToken>());

        //Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);

        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsType<Response<UserViewModel>>(badRequestResult!.Value);

        var response = badRequestResult.Value as Response<UserViewModel>;
        Assert.Equal(Messages.VAL_PASSWORD_LENGTH_NOT_VALID, response!.Errors!.First().Message);
    }

    [Theory]
    [InlineData("test1", "test@mail.com", "TestHehe")]
    [InlineData("test1", "test@mail.com", "Test1234")]
    [InlineData("test1", "test@mail.com", "test123@")]
    public async Task RegisterUserAsync_WithPasswordWithInvalidFormat_ShouldReturnErrors(string username, string email, string password)
    {
        //Arrange
        var request = new RegisterUserRequest
        {
            Username = username,
            Email = email,
            Password = password
        };
        var user = new User(request.Username, request.Email, Role.User, request.Password, []);

        var error = new Error
        {
            Message = Messages.VAL_PASSWORD_FORMAT_NOT_VALID
        };

        _mockUserService.Setup(x => x.RegisterUserAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockMapper.Setup(x => x.Map<List<Error>>(It.IsAny<List<ValidationFailure>>())).Returns([error]);

        //Act
        var result = await _userController.RegisterUserAsync(request, It.IsAny<CancellationToken>());

        //Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);

        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsType<Response<UserViewModel>>(badRequestResult!.Value);

        var response = badRequestResult.Value as Response<UserViewModel>;
        Assert.Equal(Messages.VAL_PASSWORD_FORMAT_NOT_VALID, response!.Errors!.First().Message);
    }
}