using System.Diagnostics;
using Microsoft.AspNetCore.Identity;

namespace VaxSync.Web.Data;

[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(),nq}}")]
public class ApplicationUser : IdentityUser
{
    private string DebuggerDisplay => ToString();
    public string? SchoolId { get; set; }   // now nullable
}
