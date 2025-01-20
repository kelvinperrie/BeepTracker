using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BeepTracker.Api.Models;

public partial class BeepTrackerDbContext : DbContext
{
    public BeepTrackerDbContext()
    {
    }

    public BeepTrackerDbContext(DbContextOptions<BeepTrackerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BeepEntry> BeepEntries { get; set; }

    public virtual DbSet<BeepRecord> BeepRecords { get; set; }

    public virtual DbSet<Bird> Birds { get; set; }

    public virtual DbSet<BirdStatus> BirdStatuses { get; set; }

    public virtual DbSet<Organisation> Organisations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("name=BeepTrackerConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BeepEntry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("BeepEntry_pkey");

            entity.ToTable("BeepEntry");

            entity.Property(e => e.Value).HasDefaultValue(0);

            entity.HasOne(d => d.BeepRecord).WithMany(p => p.BeepEntries)
                .HasForeignKey(d => d.BeepRecordId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BeepRecordFK");
        });

        modelBuilder.Entity<BeepRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("BeepRecord_pkey");

            entity.ToTable("BeepRecord");

            entity.Property(e => e.BeatsPerMinute).HasDefaultValue(0);

            entity.HasOne(d => d.Bird).WithMany(p => p.BeepRecords)
                .HasForeignKey(d => d.BirdId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BirdIdFK");
        });

        modelBuilder.Entity<Bird>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Birds_pkey");

            entity.ToTable("Bird");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"Birds_Id_seq\"'::regclass)");
            entity.Property(e => e.StatusId).HasDefaultValue(0);
        });

        modelBuilder.Entity<BirdStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("BirdStatus_pkey");

            entity.ToTable("BirdStatus");
        });

        modelBuilder.Entity<Organisation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Organisation_pkey");

            entity.ToTable("Organisation");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_pkey");

            entity.ToTable("User");

            entity.HasOne(d => d.Organisation).WithMany(p => p.Users)
                .HasForeignKey(d => d.OrganisationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("OrganisationFK");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
