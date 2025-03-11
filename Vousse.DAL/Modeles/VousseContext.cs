using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Vousse.DAL.Modeles;

public partial class VousseContext : DbContext
{
    public VousseContext()
    {
    }

    public VousseContext(DbContextOptions<VousseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Artiste> Artistes { get; set; }

    public virtual DbSet<Billeterie> Billeteries { get; set; }

    public virtual DbSet<Planification> Planifications { get; set; }

    public virtual DbSet<Spectacle> Spectacles { get; set; }

    public virtual DbSet<SpectacleGrouped> SpectacleGroupeds { get; set; }

    public virtual DbSet<SpectacleParent> SpectacleParents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=BDDprojetHabib;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Artiste>(entity =>
        {
            entity.HasKey(e => e.IdArtiste).HasName("PK__artiste__0728C9CFDDC46431");

            entity.ToTable("artiste");

            entity.Property(e => e.IdArtiste).HasColumnName("ID_artiste");
            entity.Property(e => e.Nom)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("nom");
        });

        modelBuilder.Entity<Billeterie>(entity =>
        {
            entity.HasKey(e => e.NumeroBillet);

            entity.ToTable("billeterie");

            entity.Property(e => e.NumeroBillet).HasColumnName("numero_billet");
            entity.Property(e => e.Civilite)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("civilite");
            entity.Property(e => e.IdSpectacle).HasColumnName("idSpectacle");
            entity.Property(e => e.Nom)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nom");
            entity.Property(e => e.Prenom)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("prenom");

            entity.HasOne(d => d.IdSpectacleNavigation).WithMany(p => p.Billeteries)
                .HasForeignKey(d => d.IdSpectacle)
                .HasConstraintName("FK__billeteri__idSpe__44FF419A");
        });

        modelBuilder.Entity<Planification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__planific__3214EC07879D4868");

            entity.ToTable("planification");

            entity.Property(e => e.DateSpectacle).HasColumnName("date_spectacle");
            entity.Property(e => e.Duree).HasColumnName("duree");
            entity.Property(e => e.IdSpectacle).HasColumnName("idSpectacle");
            entity.Property(e => e.Lieu)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("lieu");

            entity.HasOne(d => d.IdSpectacleNavigation).WithMany(p => p.Planifications)
                .HasForeignKey(d => d.IdSpectacle)
                .HasConstraintName("FK__planifica__idSpe__412EB0B6");
        });

        modelBuilder.Entity<Spectacle>(entity =>
        {
            entity.ToTable("spectacle");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.TypeDeSpectacle)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Spectacle)
                .HasForeignKey<Spectacle>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__spectacle__Id__3A81B327");
        });

        modelBuilder.Entity<SpectacleGrouped>(entity =>
        {
            entity.ToTable("spectacle_grouped");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.NombreSpectacle).HasColumnName("nombre_spectacle");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.SpectacleGrouped)
                .HasForeignKey<SpectacleGrouped>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__spectacle_gr__Id__4316F928");

            entity.HasMany(d => d.IdSpectacles).WithMany(p => p.IdSpectacleGroupeds)
                .UsingEntity<Dictionary<string, object>>(
                    "SpectacleGroupedSpectacle",
                    r => r.HasOne<Spectacle>().WithMany()
                        .HasForeignKey("IdSpectacle")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__spectacle__id_sp__6E01572D"),
                    l => l.HasOne<SpectacleGrouped>().WithMany()
                        .HasForeignKey("IdSpectacleGrouped")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__spectacle__id_sp__6D0D32F4"),
                    j =>
                    {
                        j.HasKey("IdSpectacleGrouped", "IdSpectacle").HasName("PK__spectacl__1635CCCCB96DCD8F");
                        j.ToTable("spectacle_grouped_spectacle");
                        j.IndexerProperty<int>("IdSpectacleGrouped").HasColumnName("id_spectacle_grouped");
                        j.IndexerProperty<int>("IdSpectacle").HasColumnName("id_spectacle");
                    });
        });

        modelBuilder.Entity<SpectacleParent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__spectacl__3214EC076063E2E0");

            entity.ToTable("spectacle_parent");

            entity.Property(e => e.Descriptions).HasColumnType("text");
            entity.Property(e => e.NomSpectacle)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("nomSpectacle");
            entity.Property(e => e.TarifEnfant).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TarifNormal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TarifReduit).HasColumnType("decimal(10, 2)");

            entity.HasMany(d => d.IdArtistes).WithMany(p => p.IdSpectacles)
                .UsingEntity<Dictionary<string, object>>(
                    "SpectacleArtiste",
                    r => r.HasOne<Artiste>().WithMany()
                        .HasForeignKey("IdArtiste")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__spectacle__ID_ar__3E52440B"),
                    l => l.HasOne<SpectacleParent>().WithMany()
                        .HasForeignKey("IdSpectacle")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__spectacle__Id_sp__3D5E1FD2"),
                    j =>
                    {
                        j.HasKey("IdSpectacle", "IdArtiste").HasName("PK__spectacl__A11A8D4360AD9480");
                        j.ToTable("spectacle_artiste");
                        j.IndexerProperty<int>("IdSpectacle").HasColumnName("Id_spectacle");
                        j.IndexerProperty<int>("IdArtiste").HasColumnName("ID_artiste");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
