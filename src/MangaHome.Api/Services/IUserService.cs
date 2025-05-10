using MangaHome.Api.Common;
using MangaHome.Api.Models.ViewModels;
using MangaHome.Api.Models.Requests;

namespace MangaHome.Api.Services;

public interface IUserService
{
    Task<Result<UserViewModel?, List<Error>?>> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<UserViewModel?, List<Error>?>> RegisterUserAsync(RegisterUserRequest request, CancellationToken cancellationToken);
}