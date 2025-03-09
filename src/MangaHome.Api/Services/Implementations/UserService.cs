using AutoMapper;
using MangaHome.Api.Common;
using MangaHome.Api.Models.Dtos;
using MangaHome.Api.Models.Requests;
using MangaHome.Core.Common;
using MangaHome.Core.Models;
using MangaHome.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace MangaHome.Api.Services.Implementations;

public class UserService : IUserService
{
    private readonly IDistributedCache _distributedCache;
    private readonly IMapper _mapper;
    private readonly MangaHomeDbContext _dbContext;
    private readonly IPasswordHashingService _passwordHashingService;

    public UserService(IDistributedCache distributedCache,
        IMapper mapper,
        MangaHomeDbContext dbContext,
        IPasswordHashingService passwordHashingService)
    {
        _distributedCache = distributedCache;
        _mapper = mapper;
        _dbContext = dbContext;
        _passwordHashingService = passwordHashingService;
    }

    public async Task<Result<UserDto?, List<Error>?>> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var key = $"user-{id}";
        var cachedUser = await _distributedCache.GetStringAsync(key, cancellationToken);
        User? user;
        if (string.IsNullOrEmpty(cachedUser))
        {
            user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (user == null)
            {
                return new List<Error>
                {
                    new() {
                        Type = ErrorType.Error.ToString().ToLower(),
                        Message = Messages.ERR_USER_NOT_FOUND,
                    }
                };
            }
            await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(user), cancellationToken);
            return _mapper.Map<UserDto>(user);
        }
        user = JsonSerializer.Deserialize<User>(cachedUser);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<Result<UserDto?, List<Error>?>> RegisterUserAsync(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var errors = new List<Error>();

        var existingUserWithSameEmail = await _dbContext.Users.FirstOrDefaultAsync(
            u => u.Email == request.Email,
            cancellationToken: cancellationToken);

        if (existingUserWithSameEmail is not null)
        {
            errors.Add(new Error
            {
                Type = ErrorType.Validation.ToString().ToLower(),
                Message = Messages.VAL_EMAIL_ALREADY_EXISTED
            });
        }

        var existingUserWithSameUsername = await _dbContext.Users.FirstOrDefaultAsync(
            u => u.Username == request.Username,
            cancellationToken: cancellationToken);

        if (existingUserWithSameUsername is not null)
        {
            errors.Add(new Error
            {
                Type = ErrorType.Validation.ToString().ToLower(),
                Message = Messages.VAL_USERNAME_ALREADY_EXISTED
            });
        }

        if (errors.Count > 0) return errors;

        (string hashed, byte[] salt) = _passwordHashingService.HashPassword(request.Password!);
        var user = new User(
            request.Username!,
            request.Email!,
            Role.User,
            hashed,
            salt
        );
        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UserDto>(user);
    }
}