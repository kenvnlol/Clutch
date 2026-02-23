using Clutch.Database;
using Clutch.Database.Entities.Users;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Clutch.API.Features.Users.Endpoints;



[Handler]
[MapPost("/users/login")]
public static partial class Login
{
    public sealed record Command(
        string UserName,
        string Password,
        bool RememberMe);
    public sealed record LoginResponse(
        string UserId,
        bool IsLoggedIn,
        IReadOnlyList<UserEntitlement> UserEntitlements);
    public sealed record UserEntitlement(
        string EntitlementKey,
        string EntitlementValue);
    public sealed record RestaurantDetails(
        int Id,
        string Name,
        string Role);

    private static async ValueTask<LoginResponse> HandleAsync(
        Command command,
        ApplicationDbContext dbContext,
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        CancellationToken token)
    {
        var result = await signInManager.PasswordSignInAsync(
            command.UserName,
            command.Password,
            command.RememberMe,
            false);

        if (!result.Succeeded)
        {
            throw new UnauthorizedAccessException();
        }


        else
        {
            if (result.IsLockedOut)
            {
                throw new UnauthorizedAccessException("Too many failed login attempts. Account is temporarily locked.");
            }
        }

        var user = await dbContext.Users
            .FirstAsync();

        var userRoles = await userManager.GetClaimsAsync(user);

        return new LoginResponse(
            user.Id,
            true,
            userRoles.Select(x => new UserEntitlement(
                x.Type,
                x.Value)).ToList());
    }
}
