using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RN_International_Website.Models
{
    

    public class ApplicationDbContext : IdentityDbContext<Users>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Define a DbSet for TeamMember
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
    }

}
