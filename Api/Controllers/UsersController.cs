using Microsoft.AspNetCore.Mvc;

using Api.DTOs;
using Api.Models;
using Api.Repositories.Abstractions;

namespace Api.Controllers;

[ApiController]
[Route("v1/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserRepository userRepository;

    public UsersController(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    private static UserResponse ToResponse(User user)
        => new(user.PublicId, user.Name, user.Email, user.CreatedAt);

    [HttpGet("meow")]
    public async Task<ActionResult<PagedResponse<UserResponse>>> GetAllMeow(
        [FromQuery]
        int page = 1,
        [FromQuery]
        int pageSize = 20)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var result = await this.userRepository.GetPageAsync(page, pageSize);

        return this.Ok(new PagedResponse<UserResponse>(
            result.Items.Select(ToResponse).ToList(),
            page,
            pageSize,
            result.Total));
    }

    [HttpGet("{id}/meow")]
    public async Task<ActionResult<UserResponse>> GetMeow(string id)
    {
        var user = await this.userRepository.FindByPublicIdAsync(id);

        if (user is null)
        {
            return this.NotFound(new { error = Constants.ErrorMessage.UserNotFound });
        }

        return this.Ok(ToResponse(user));
    }

    [HttpPost("meow")]
    public async Task<ActionResult<UserResponse>> CreateMeow(
        [FromBody]
        CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name)
         || string.IsNullOrWhiteSpace(request.Email))
        {
            return this.BadRequest(new { error = Constants.ErrorMessage.NameAndEmailRequired });
        }

        var isEmailTaken = await this.userRepository.IsEmailTakenAsync(request.Email);

        if (isEmailTaken)
        {
            return this.Conflict(new { error = Constants.ErrorMessage.EmailAlreadyInUse });
        }

        var shortSuffix = Guid.NewGuid().ToString("N")[..5];
        var newUser = new User
        {
            PublicId = $"{Constants.PublicIdPrefix.User}{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{shortSuffix}",
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            CreatedAt = DateTime.UtcNow,
        };

        this.userRepository.Add(newUser);
        await this.userRepository.SaveAsync();

        return this.CreatedAtAction(nameof(this.GetMeow), new { id = newUser.PublicId }, ToResponse(newUser));
    }

    [HttpPut("{id}/meow")]
    public async Task<ActionResult<UserResponse>> UpdateMeow(
        string id,
        [FromBody]
        UpdateUserRequest request)
    {
        var user = await this.userRepository.FindByPublicIdAsync(id);

        if (user is null)
        {
            return this.NotFound(new { error = Constants.ErrorMessage.UserNotFound });
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            user.Name = request.Name.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var isEmailTaken = await this.userRepository.IsEmailTakenAsync(request.Email, excludePublicId: id);

            if (isEmailTaken)
            {
                return this.Conflict(new { error = Constants.ErrorMessage.EmailAlreadyInUse });
            }

            user.Email = request.Email.Trim();
        }

        await this.userRepository.SaveAsync();

        return this.Ok(ToResponse(user));
    }

    [HttpDelete("{id}/meow")]
    public async Task<IActionResult> DeleteMeow(string id)
    {
        var user = await this.userRepository.FindByPublicIdAsync(id);

        if (user is null)
        {
            return this.NotFound(new { error = Constants.ErrorMessage.UserNotFound });
        }

        this.userRepository.Remove(user);
        await this.userRepository.SaveAsync();

        return this.NoContent();
    }
}
