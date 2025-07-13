using MangaHome.Api.Common;
using MangaHome.Api.Exceptions;
using MangaHome.Api.Models.Requests;
using MangaHome.Core.Values;
using MangaHome.Core.Models;
using MangaHome.Infrastructure.Contexts;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Security.Cryptography;

namespace MangaHome.Api.Services.Implementations;

public class UserService : IUserService
{
    private readonly MangaHomeDbContext _dbContext;
    private readonly IDistributedCache _distributedCache;

    public UserService(
        MangaHomeDbContext dbContext,
        IDistributedCache distributedCache
    )
    {
        _dbContext = dbContext;
        _distributedCache = distributedCache;
    }

    public async Task<User> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var key = $"user-{id}";
        var cachedUser = await _distributedCache.GetStringAsync(key, cancellationToken);
        User? user;
        if (string.IsNullOrEmpty(cachedUser))
        {
            user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException(Messages.ERR_USER_NOT_FOUND);
            }
            await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(user), cancellationToken);
            return user;
        }
        user = JsonSerializer.Deserialize<User>(cachedUser);
        return user!;
    }

    public async Task<User> RegisterUserAsync(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var existingUserWithSameEmail = await _dbContext.Users.FirstOrDefaultAsync(
            u => u.Email == request.Email,
            cancellationToken: cancellationToken);

        if (existingUserWithSameEmail is not null)
        {
            throw new BadRequestException(Messages.VAL_EMAIL_ALREADY_EXISTED);
        }

        var existingUserWithSameUsername = await _dbContext.Users.FirstOrDefaultAsync(
            u => u.Username == request.Username,
            cancellationToken: cancellationToken);

        if (existingUserWithSameUsername is not null)
        {
            throw new BadRequestException(Messages.VAL_USERNAME_ALREADY_EXISTED);
        }

        (string hashed, byte[] salt) = HashPassword(request.Password!);
        var user = new User(
            request.Username!,
            request.Email!,
            Role.User,
            hashed,
            salt
        );
        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return user;
    }

    private static (string hashedPassword, byte[] salt) HashPassword(string password, byte[]? salt = null)
    {
        salt ??= RandomNumberGenerator.GetBytes(128 / 8);
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
        
        return (hashed, salt);
    }
}