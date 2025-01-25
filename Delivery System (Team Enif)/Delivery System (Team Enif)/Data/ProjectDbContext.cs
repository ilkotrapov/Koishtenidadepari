<<<<<<< Updated upstream
<<<<<<< Updated upstream
﻿using Microsoft.EntityFrameworkCore;
=======
using System;
>>>>>>> Stashed changes
=======
using System;
>>>>>>> Stashed changes
using Delivery_System__Team_Enif_.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Delivery_System__Team_Enif_.Data
{
    public class ProjectDbContext : DbContext
    {
        public DbSet<Package> package { get; set; }
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) { }
<<<<<<< Updated upstream
=======

        public DbSet<User> Users { get; set; }
        public DbSet<Delivery_System__Team_Enif_.Data.Entities.Package> Packages { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Office> Offices { get; set; }
        public DbSet<Courier> Couriers { get; set; }    
>>>>>>> Stashed changes
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
