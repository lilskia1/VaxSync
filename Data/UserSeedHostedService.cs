using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VaxSync.Web.Data;

namespace VaxSync.Web.Data;

public sealed class UserSeedHostedService : IHostedService
{
    private readonly IServiceProvider _sp;

    public UserSeedHostedService(IServiceProvider sp) => _sp = sp;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _sp.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Define roles
        string[] roles = ["Admin", "SchoolNurse", "Viewer"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Seed users
        await EnsureUserAsync(userManager, "admin@vaxsync.local", "Admin123!", "Admin");
        await EnsureUserAsync(userManager, "nurse@vaxsync.local", "Nurse123!", "SchoolNurse");
        await EnsureUserAsync(userManager, "viewer@vaxsync.local", "Viewer123!", "Viewer");
    }

    private static async Task EnsureUserAsync(UserManager<ApplicationUser> userManager, string email, string password, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                SchoolId = role == "SchoolNurse" ? "SCH0001" : null
            };
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new Exception($"Failed to create {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        if (!await userManager.IsInRoleAsync(user, role))
            await userManager.AddToRoleAsync(user, role);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
