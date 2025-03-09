using AutoMapper;
using FluentValidation.Results;
using MangaHome.Api.Models.Dtos;
using MangaHome.Core.Models;

namespace MangaHome.Api.Common;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<ValidationFailure, Error>()
            .ForMember(err => err.Message, act => act.MapFrom(src => src.ErrorMessage))
            .ForMember(err => err.Type, act => act.MapFrom(src => ErrorType.Validation.ToString().ToLower()));

        CreateMap<User, UserDto>();
    }
}