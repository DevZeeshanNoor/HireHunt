using HireHuntBackend.Model;
using Microsoft.EntityFrameworkCore;

namespace HireHuntBackend.Context
{
    public class DbContextHireHunt:DbContext
    {
        public DbContextHireHunt(DbContextOptions<DbContextHireHunt> options) : base(options) { }
        public DbSet<JobPost> JobPosts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Recruiter> Recruiters {  get; set; }
    }
}
