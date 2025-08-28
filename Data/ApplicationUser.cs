using System.Diagnostics;
using Microsoft.AspNetCore.Identity;

namespace VaxSync.Web.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(),nq}}")]
public class ApplicationUser : IdentityUser
{
    private string DebuggerDisplay => ToString();
}

