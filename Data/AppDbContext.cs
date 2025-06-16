using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CarManagment.Models;

namespace CarManagment.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<WorkType> WorkTypes { get; set; }
        public DbSet<RepairWork> Repairs { get; set; }
        public DbSet<Repair> RepairOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Vehicle>(entity =>
            {
                entity.HasOne(v => v.Owner)
                    .WithMany()
                    .HasForeignKey(v => v.OwnerId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(v => v.Repairs)
                    .WithOne(rw => rw.Vehicle)
                    .HasForeignKey(rw => rw.VehicleId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<WorkType>(entity =>
            {
                entity.HasMany(w => w.Repairs)
                    .WithOne(rw => rw.WorkType)
                    .HasForeignKey(rw => rw.WorkTypeId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<RepairWork>(entity =>
            {
                entity.HasOne(rw => rw.Vehicle)
                    .WithMany(v => v.Repairs)
                    .HasForeignKey(rw => rw.VehicleId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rw => rw.WorkType)
                    .WithMany(w => w.Repairs)
                    .HasForeignKey(rw => rw.WorkTypeId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Repair>(entity =>
            {
                entity.HasOne(r => r.Vehicle)
                    .WithMany()
                    .HasForeignKey(r => r.VehicleId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(r => r.RepairWorks)
                    .WithOne(rw => rw.Repair)
                    .HasForeignKey(rw => rw.RepairId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
