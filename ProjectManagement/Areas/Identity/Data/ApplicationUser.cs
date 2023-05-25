using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;

namespace ProjectManagement.Areas.Identity.Data
{
    public class ApplicationUser: IdentityUser
    {
        [Required]
        public string? Name { get; set; }
    }
}
