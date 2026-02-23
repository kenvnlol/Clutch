using Clutch.Database.Entities.Users;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Clutch.Features.Users.Services;

public class UserService(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, SignInManager<User> signInManager)
{
    public bool IsLoggedIn()
        => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;
    public string GetCurrentUserId()
    {
        if (!IsLoggedIn())
        {
            // Is this redundant? wont the exception be thrown in IsLoggedIn?
            throw new UnauthorizedAccessException("Cannot get current user ID. User is not authenticated.");
        }

        return httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }

    public async Task<User?> GetCurrentUser()
        => await userManager.FindByIdAsync(GetCurrentUserId());
}
