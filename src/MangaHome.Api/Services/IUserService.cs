using MangaHome.Api.Common;
using MangaHome.Api.Models.Dtos;
using MangaHome.Api.Models.Requests;

namespace MangaHome.Api.Services;

public interface IUserService
{
    Task<Result<UserDto?, List<Error>?>> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<UserDto?, List<Error>?>> RegisterUserAsync(RegisterUserRequest request, CancellationToken cancellationToken);
}