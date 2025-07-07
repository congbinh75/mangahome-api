using AutoMapper;
using MangaHome.Api.Common;
using MangaHome.Api.Models.ViewModels;
using MangaHome.Api.Models.Requests;
using MangaHome.Api.Models.Responses;
using MangaHome.Api.Services;
using MangaHome.Api.Validators;
using Microsoft.AspNetCore.Mvc;

namespace MangaHome.Api.Controllers;

[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public UserController(
        IMapper mapper, 
        IUserService userService
    )
    {
        _mapper = mapper;
        _userService = userService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserByIdAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var user = await _userService.GetUserByIdAsync(id, cancellationToken);

        return Ok(new Response<UserViewModel>
        {
            Data = _mapper.Map<UserViewModel>(user),
        });
    }

    [HttpPost]
    public async Task<IActionResult> RegisterUserAsync(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken
    )
    {
        var validator = new RegisterUserValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(new Response<UserViewModel>
            {
                Success = false,
                Errors = _mapper.Map<List<Error>>(validationResult.Errors)
            });
        }

        var user = await _userService.RegisterUserAsync(request, cancellationToken);

        return StatusCode(201, new Response<string>
        {
            Data = user.Id.ToString(),
        });
    }
}