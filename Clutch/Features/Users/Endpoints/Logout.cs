using Clutch.Database.Entities.Users;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Identity;

namespace Clutch.API.Features.Users.Endpoints;

[Handler]
[MapPost("/user/logout")]
public static partial class Logout
{
    public sealed record Query;
    private static async ValueTask HandleAsync(
        Query _,
        SignInManager<User> signInManager,
        CancellationToken token)
            => await signInManager.SignOutAsync();
}
