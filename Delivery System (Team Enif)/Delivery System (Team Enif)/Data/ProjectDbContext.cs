
﻿using Microsoft.EntityFrameworkCore;
using System;
using System;
using Delivery_System__Team_Enif_.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Delivery_System__Team_Enif_.Models;

namespace Delivery_System__Team_Enif_.Data
{
    public class ProjectDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) { }
        public DbSet<Package> Package { get; set; }
        
        public DbSet<Package> Packages { get; set; }

        public DbSet<DeliveryType> DeliveryTypes { get; set; }
        public DbSet<DeliveryStatus> DeliveryStatuses { get; set; }

        public DbSet<DeliveryOption> DeliveryOptions { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Office> Offices { get; set; }
        public DbSet<Courier> Couriers { get; set; }    
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
