using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Somokat;

public partial class SomokatContext : DbContext
{
    public SomokatContext()
    {
    }

    public SomokatContext(DbContextOptions<SomokatContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChargingStation> ChargingStations { get; set; }

    public virtual DbSet<Complaint> Complaints { get; set; }

    public virtual DbSet<LocationJournal> LocationJournals { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Scooter> Scooters { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=hnt8.ru;Port=5432;Database=somokat;Username=admin;Password=admin");
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChargingStation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("charging_station_pkey");

            entity.ToTable("charging_station");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AvailableChargers).HasColumnName("available_chargers");
            entity.Property(e => e.Location).HasColumnName("location");
        });

        modelBuilder.Entity<Complaint>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("complaint_pkey");

            entity.ToTable("complaint");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ImageLink)
                .HasMaxLength(255)
                .HasColumnName("image_link");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Time)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("time");

            entity.HasOne(d => d.Order).WithMany(p => p.Complaints)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("complaint_order_id_fkey");
        });

        modelBuilder.Entity<LocationJournal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("locationjournal_pkey");

            entity.ToTable("location_journal");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.ScooterId).HasColumnName("scooter_id");
            entity.Property(e => e.Time)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("time");

            entity.HasOne(d => d.Scooter).WithMany(p => p.LocationJournals)
                .HasForeignKey(d => d.ScooterId)
                .HasConstraintName("locationjournal_scooter_id_fkey");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Order_pkey");

            entity.ToTable("Order");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EndTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("end_time");
            entity.Property(e => e.OrderTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("order_time");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.ScooterId).HasColumnName("scooter_id");
            entity.Property(e => e.StartTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("start_time");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Scooter).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ScooterId)
                .HasConstraintName("Order_scooter_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Order_user_id_fkey");
        });

        modelBuilder.Entity<Scooter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("scooter_pkey");

            entity.ToTable("scooter");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BatteryLevel).HasColumnName("battery_level");
            entity.Property(e => e.IdStatus).HasColumnName("id_status");
            entity.Property(e => e.Location).HasColumnName("location");

            entity.HasOne(d => d.IdStatusNavigation).WithMany(p => p.Scooters)
                .HasForeignKey(d => d.IdStatus)
                .HasConstraintName("scooter_id_status_fkey");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("status_pkey");

            entity.ToTable("status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Status1)
                .HasMaxLength(20)
                .HasColumnName("status");
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("useraccount_pkey");

            entity.ToTable("user_account");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Bonus).HasColumnName("bonus");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
