using Microsoft.AspNetCore.Identity;

namespace RN_International_Website.Models
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }
    }
}
