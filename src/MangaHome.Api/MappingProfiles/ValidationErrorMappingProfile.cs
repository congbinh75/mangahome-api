using AutoMapper;
using FluentValidation.Results;
using MangaHome.Api.Common;

namespace MangaHome.Api.MappingProfiles;

public class ValidationErrorMappingProfile : Profile
{
    public ValidationErrorMappingProfile()
    {
        CreateMap<ValidationFailure, Error>()
            .ForMember(err => err.Message, act => act.MapFrom(src => src.ErrorMessage));
    }
}