using AutoMapper;
using FluentValidation.Results;
using MangaHome.Api.Common;

namespace MangaHome.Api.MappingProfiles;

public class CommonMappingProfile : Profile
{
    public CommonMappingProfile()
    {
        CreateMap<ValidationFailure, Error>()
            .ForMember(err => err.Message, act => act.MapFrom(src => src.ErrorMessage));
    }
}