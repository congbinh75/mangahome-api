using AutoMapper;
using MangaHome.Api.Common;
using MangaHome.Api.Models.Dtos;
using MangaHome.Api.Models.Requests;
using MangaHome.Api.Models.Responses;
using MangaHome.Api.Services;
using MangaHome.Api.Validators;
using Microsoft.AspNetCore.Mvc;

namespace MangaHome.Api.Controllers;

[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public UserController(IMapper mapper, IUserService userService)
    {
        _mapper = mapper;
        _userService = userService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _userService.GetUserByIdAsync(id, cancellationToken);
        if (result.Success)
        {
            return StatusCode(200, new Response<UserDto>
            {
                Data = result.Value,
            });
        }
        else
        {
            return BadRequest(new Response<UserDto>
            {
                Success = false,
                Errors = result.Errors,
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> RegisterUserAsync(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var validator = new RegisterUserValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(new Response<UserDto>
            {
                Success = false,
                Errors = _mapper.Map<List<Error>>(validationResult.Errors)
            });
        }

        var result = await _userService.RegisterUserAsync(request, cancellationToken);
        if (result.Success)
        {
            return StatusCode(201, new Response<string>
            {
                Data = result.Value!.Id,
            });
        }
        else
        {
            return BadRequest(new Response<UserDto>
            {
                Success = false,
                Errors = result.Errors,
            });
        }
    }
}