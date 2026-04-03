using Api.Data;
using Api.DTOs;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("v1/users")]
public sealed class UsersController : ControllerBase
{
    private readonly AppDbContext database;

    public UsersController(AppDbContext database)
    {
        this.database = database;
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

        var total = await this.database.Users.CountAsync();
        var items = await this.database.Users
            .OrderBy(user => user.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(user => ToResponse(user))
            .ToListAsync();

        return this.Ok(new PagedResponse<UserResponse>(items, page, pageSize, total));
    }

    [HttpGet("{id}/meow")]
    public async Task<ActionResult<UserResponse>> GetMeow(string id)
    {
        var user = await this.database.Users
            .FirstOrDefaultAsync(user => string.Equals(user.PublicId, id, StringComparison.Ordinal));

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

        var isEmailTaken = await this.database.Users
            .AnyAsync(user => string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase));

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

        this.database.Users.Add(newUser);
        await this.database.SaveChangesAsync();

        return this.CreatedAtAction(nameof(this.GetMeow), new { id = newUser.PublicId }, ToResponse(newUser));
    }

    [HttpPut("{id}/meow")]
    public async Task<ActionResult<UserResponse>> UpdateMeow(
        string id,
        [FromBody]
        UpdateUserRequest request)
    {
        var user = await this.database.Users
            .FirstOrDefaultAsync(user => string.Equals(user.PublicId, id, StringComparison.Ordinal));

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
            var isEmailTaken = await this.database.Users
                .AnyAsync(other => string.Equals(other.Email, request.Email, StringComparison.OrdinalIgnoreCase)
                                && !string.Equals(other.PublicId, id, StringComparison.Ordinal));

            if (isEmailTaken)
            {
                return this.Conflict(new { error = Constants.ErrorMessage.EmailAlreadyInUse });
            }

            user.Email = request.Email.Trim();
        }

        await this.database.SaveChangesAsync();

        return this.Ok(ToResponse(user));
    }

    [HttpDelete("{id}/meow")]
    public async Task<IActionResult> DeleteMeow(string id)
    {
        var user = await this.database.Users
            .FirstOrDefaultAsync(user => string.Equals(user.PublicId, id, StringComparison.Ordinal));

        if (user is null)
        {
            return this.NotFound(new { error = Constants.ErrorMessage.UserNotFound });
        }

        this.database.Users.Remove(user);
        await this.database.SaveChangesAsync();

        return this.NoContent();
    }
}
