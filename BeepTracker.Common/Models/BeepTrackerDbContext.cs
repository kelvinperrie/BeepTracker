using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BeepTracker.Common.Models;

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

    public virtual DbSet<OrganisationUserRole> OrganisationUserRoles { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

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
            entity.Property(e => e.DateSaved).HasColumnType("timestamp without time zone");
            entity.Property(e => e.RecordedDateTime).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.Bird).WithMany(p => p.BeepRecords)
                .HasForeignKey(d => d.BirdId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BirdIdFK");

            entity.HasOne(d => d.User).WithMany(p => p.BeepRecords)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("UserIdFK");
        });

        modelBuilder.Entity<Bird>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Birds_pkey");

            entity.ToTable("Bird");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"Birds_Id_seq\"'::regclass)");
            entity.Property(e => e.StatusId).HasDefaultValue(0);

            entity.HasOne(d => d.Organisation).WithMany(p => p.Birds)
                .HasForeignKey(d => d.OrganisationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKOrganisationId");

            entity.HasOne(d => d.Status).WithMany(p => p.Birds)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKBirdStatusId");
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

        modelBuilder.Entity<OrganisationUserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.OrganisationId }).HasName("OrganisationUser_pkey");

            entity.ToTable("OrganisationUserRole");

            entity.Property(e => e.Active).HasDefaultValue(true);

            entity.HasOne(d => d.Organisation).WithMany(p => p.OrganisationUserRoles)
                .HasForeignKey(d => d.OrganisationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKOrganisationId");

            entity.HasOne(d => d.Role).WithMany(p => p.OrganisationUserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKRoleId");

            entity.HasOne(d => d.User).WithMany(p => p.OrganisationUserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKUserId");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Role_pkey");

            entity.ToTable("Role");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_pkey");

            entity.ToTable("User");

            entity.HasIndex(e => e.Username, "User_Username_Username1_key").IsUnique();

            entity.Property(e => e.Active).HasDefaultValue(true);

            entity.HasOne(d => d.Organisation).WithMany(p => p.Users)
                .HasForeignKey(d => d.OrganisationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("OrganisationFK");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
