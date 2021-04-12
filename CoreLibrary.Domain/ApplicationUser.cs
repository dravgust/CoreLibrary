using Microsoft.AspNetCore.Identity;

namespace CoreLibrary.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; }
    }
}