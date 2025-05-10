// **AI** i want to add an authentication to this razor page project. 
//follow these steps (steps provided in the lecture notes) and integrate authentication into my codes. 
//make sure already-existing functionalities remain working:

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Models;

namespace MyRazorApp.Data
{
    public class SchoolDbContext : IdentityDbContext<ApplicationUser>
    {
        public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
            : base(options)
        {
        }

        public DbSet<Class> Classes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Class>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId);
        }
    }
}