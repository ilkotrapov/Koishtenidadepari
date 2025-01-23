using Microsoft.EntityFrameworkCore;
using Delivery_System__Team_Enif_.Data.Entities;

namespace Delivery_System__Team_Enif_.Data
{
    public class ProjectDbContext : DbContext
    {
        public DbSet<Package> package { get; set; }
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
