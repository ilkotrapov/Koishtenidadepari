
﻿using Microsoft.EntityFrameworkCore;
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

        public DbSet<Package> Packages { get; set; }
        public DbSet<DeliveryType> DeliveryTypes { get; set; }
        public DbSet<DeliveryStatus> DeliveryStatuses { get; set; }
        public DbSet<DeliveryOption> DeliveryOptions { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Office> Offices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DeliveryStatus>().HasData(
                new DeliveryStatus { Id = 1, Name = "Pending" },
                new DeliveryStatus { Id = 2, Name = "Active" },
                new DeliveryStatus { Id = 3, Name = "Completed" }
            );

            modelBuilder.Entity<DeliveryType>().HasData(
                new DeliveryType { Id = 1, Name = "Standard" },
                new DeliveryType { Id = 2, Name = "Express" }
            );

            modelBuilder.Entity<DeliveryOption>().HasData(
                new DeliveryOption { Id = 1, Name = "DoorToDoor" },
                new DeliveryOption { Id = 2, Name = "PickUp_DropOffLocalOffice" }
            );
        }
    }

}
