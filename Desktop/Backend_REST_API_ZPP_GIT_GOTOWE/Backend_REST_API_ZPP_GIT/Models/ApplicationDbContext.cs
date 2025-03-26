using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Backend_REST_API_ZPP.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Kierunek> Kieruneks { get; set; }

    // Исправлено на правильный тип DbSet<Prowadzacy>
    public virtual DbSet<Prowadzacy> Prowadzacies { get; set; }

    public virtual DbSet<Przedmiot> Przedmiots { get; set; }

    public virtual DbSet<Sala> Salas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=localhost;Database=SCRAPER;Trusted_Connection=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Polish_CI_AS");

        modelBuilder.Entity<Kierunek>(entity =>
        {
            entity.HasKey(e => e.IdKierunek).HasName("PK__Kierunek__9FEFDE1E8AD0DBD7");

            entity.ToTable("Kierunek");

            entity.Property(e => e.IdKierunek).HasColumnName("ID_Kierunek");
            entity.Property(e => e.Nazwa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TypKierunku)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Typ_Kierunku");
        });

        modelBuilder.Entity<Prowadzacy>(entity =>
        {
            entity.HasKey(e => e.IdProwadzacy).HasName("PK__Prowadza__8EF96D06400BC302");

            entity.ToTable("Prowadzacy");

            entity.Property(e => e.IdProwadzacy).HasColumnName("ID_Prowadzacy");
            entity.Property(e => e.Imie)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nazwisko)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Skrot)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Tytul)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Przedmiot>(entity =>
        {
            entity.HasKey(e => e.IdTowaru).HasName("PK__Przedmio__B4EDE03199A8138C");

            entity.ToTable("Przedmiot");

            entity.Property(e => e.IdTowaru).HasColumnName("ID_Towaru");
            entity.Property(e => e.IdKierunek).HasColumnName("ID_Kierunek");
            entity.Property(e => e.IdProwadzacy).HasColumnName("ID_Prowadzacy");
            entity.Property(e => e.IdSala).HasColumnName("ID_Sala");
            entity.Property(e => e.Nazwa)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Skrot)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Typ)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdKierunekNavigation).WithMany(p => p.Przedmiots)
                .HasForeignKey(d => d.IdKierunek)
                .HasConstraintName("FK__Przedmiot__ID_Ki__3F466844");

            entity.HasOne(d => d.IdProwadzacyNavigation).WithMany(p => p.Przedmiots)
                .HasForeignKey(d => d.IdProwadzacy)
                .HasConstraintName("FK__Przedmiot__ID_Pr__3D5E1FD2");

            entity.HasOne(d => d.IdSalaNavigation).WithMany(p => p.Przedmiots)
                .HasForeignKey(d => d.IdSala)
                .HasConstraintName("FK__Przedmiot__ID_Sa__3E52440B");
        });

        modelBuilder.Entity<Sala>(entity =>
        {
            entity.HasKey(e => e.IdSala).HasName("PK__Sala__2071DEA7ECDCAAA0");

            entity.ToTable("Sala");

            entity.Property(e => e.IdSala).HasColumnName("ID_Sala");
            entity.Property(e => e.Budynek)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Pomieszczenie)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
