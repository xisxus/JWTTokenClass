using Microsoft.AspNetCore.Identity;

namespace ApiWIthJWTAuth.Data
{
    public class ApplicationUser:IdentityUser
    {
        public string? Name { get; set; }
    }

}
