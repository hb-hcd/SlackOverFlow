using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NotQuora.Models;

namespace NotQuora.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<Question> Questions {get; set;}
        public DbSet<Answer> Answers {get; set;}

        public DbSet<Tag> Tags {get; set;}
    }
}