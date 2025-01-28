
﻿using Microsoft.EntityFrameworkCore;
using System;
using System;
using Delivery_System__Team_Enif_.Data.Entities;

namespace Delivery_System__Team_Enif_.Data
{
    public class ProjectDbContext : DbContext
    {
        public DbSet<Package> package { get; set; }
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Delivery_System__Team_Enif_.Data.Entities.Package> Packages { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Office> Offices { get; set; }
        public DbSet<Courier> Couriers { get; set; }    
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
