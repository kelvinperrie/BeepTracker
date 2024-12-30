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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("name=BeepTrackerConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BeepEntry>(entity =>
        {
            entity.ToTable("BeepEntry");

            entity.HasOne(d => d.BeepRecord).WithMany(p => p.BeepEntries)
                .HasForeignKey(d => d.BeepRecordId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BeepEntry_BeepRecord");
        });

        modelBuilder.Entity<BeepRecord>(entity =>
        {
            entity.ToTable("BeepRecord");

            entity.Property(e => e.BirdName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ClientGeneratedKey)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Filename)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Latitude)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Longitude)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.RecordedDateTime).HasColumnType("datetime");

            entity.HasOne(d => d.Bird).WithMany(p => p.BeepRecords)
                .HasForeignKey(d => d.BirdId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BeepRecord_Bird");
        });

        modelBuilder.Entity<Bird>(entity =>
        {
            entity.ToTable("Bird");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
