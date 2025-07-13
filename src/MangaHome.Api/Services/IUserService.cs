using MangaHome.Api.Models.Requests;
using MangaHome.Core.Models;

namespace MangaHome.Api.Services;

public interface IUserService
{
    Task<User> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<User> RegisterUserAsync(RegisterUserRequest request, CancellationToken cancellationToken);
}