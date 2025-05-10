using AutoMapper;
using MangaHome.Api.Models.ViewModels;
using MangaHome.Core.Models;

namespace MangaHome.Api.MappingProfiles;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserViewModel>();
    }
}