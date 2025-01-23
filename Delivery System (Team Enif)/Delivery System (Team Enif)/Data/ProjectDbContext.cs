using System;
﻿using Microsoft.EntityFrameworkCore;
using Delivery_System__Team_Enif_.Data.Entities;

namespace Delivery_System__Team_Enif_.Data
{
    public class ProjectDbContext : DbContext
    {
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Office> Offices { get; set; }
        public DbSet<Courier> Couriers { get; set; }    
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
