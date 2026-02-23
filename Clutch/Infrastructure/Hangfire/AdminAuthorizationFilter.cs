using Hangfire.Dashboard;

namespace Clutch.Infrastructure.Hangfire;
// TODO: swap out admin to create a policy. 
public sealed class AdminAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) =>
        context.GetHttpContext().User
            .HasClaim("Role", "Admin");
}
