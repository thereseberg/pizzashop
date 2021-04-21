using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace TomasosPizzeria.Models
{
    public partial class TomasosContext : DbContext
    {
        public TomasosContext()
        {
        }

        public TomasosContext(DbContextOptions<TomasosContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }
        public virtual DbSet<Bestallning> Bestallnings { get; set; }
        public virtual DbSet<BestallningMatratt> BestallningMatratts { get; set; }
        public virtual DbSet<Matratt> Matratts { get; set; }
        public virtual DbSet<MatrattProdukt> MatrattProdukts { get; set; }
        public virtual DbSet<MatrattTyp> MatrattTyps { get; set; }
        public virtual DbSet<Produkt> Produkts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=localhost; Database=Tomasos ;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Finnish_Swedish_CI_AS_KS_WS");

            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetRoleClaim>(entity =>
            {
                entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.Gatuadress)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Namn)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.Points).HasDefaultValueSql("((0))");

                entity.Property(e => e.Postnr)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Postort)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaim>(entity =>
            {
                entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId, "IX_AspNetUserRoles_RoleId");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<Bestallning>(entity =>
            {
                entity.HasKey(e => e.BestallningId);

                entity.ToTable("Bestallning");

                entity.Property(e => e.BestallningId).HasColumnName("BestallningID");

                entity.Property(e => e.BestallningDatum).HasColumnType("datetime");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.Bestallnings)
                    .HasForeignKey(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bestallning_AspNetUsers");
            });

            modelBuilder.Entity<BestallningMatratt>(entity =>
            {
                entity.HasKey(e => new { e.MatrattId, e.BestallningId });

                entity.ToTable("BestallningMatratt");

                entity.Property(e => e.MatrattId).HasColumnName("MatrattID");

                entity.Property(e => e.BestallningId).HasColumnName("BestallningID");

                entity.Property(e => e.Antal).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Bestallning)
                    .WithMany(p => p.BestallningMatratts)
                    .HasForeignKey(d => d.BestallningId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BestallningMatratt_Bestallning");

                entity.HasOne(d => d.Matratt)
                    .WithMany(p => p.BestallningMatratts)
                    .HasForeignKey(d => d.MatrattId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BestallningMatratt_Matratt");
            });

            modelBuilder.Entity<Matratt>(entity =>
            {
                entity.ToTable("Matratt");

                entity.Property(e => e.MatrattId).HasColumnName("MatrattID");

                entity.Property(e => e.Beskrivning)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.MatrattNamn)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MatrattTypId).HasColumnName("MatrattTypID");

                entity.HasOne(d => d.MatrattTyp)
                    .WithMany(p => p.Matratts)
                    .HasForeignKey(d => d.MatrattTypId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Matratt_MatrattTyp");
            });

            modelBuilder.Entity<MatrattProdukt>(entity =>
            {
                entity.HasKey(e => new { e.MatrattId, e.ProduktId });

                entity.ToTable("MatrattProdukt");

                entity.Property(e => e.MatrattId).HasColumnName("MatrattID");

                entity.Property(e => e.ProduktId).HasColumnName("ProduktID");

                entity.HasOne(d => d.Matratt)
                    .WithMany(p => p.MatrattProdukts)
                    .HasForeignKey(d => d.MatrattId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MatrattProdukt_Matratt");

                entity.HasOne(d => d.Produkt)
                    .WithMany(p => p.MatrattProdukts)
                    .HasForeignKey(d => d.ProduktId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MatrattProdukt_Produkt");
            });

            modelBuilder.Entity<MatrattTyp>(entity =>
            {
                entity.ToTable("MatrattTyp");

                entity.Property(e => e.MatrattTypId).HasColumnName("MatrattTypID");

                entity.Property(e => e.Beskrivning)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Produkt>(entity =>
            {
                entity.ToTable("Produkt");

                entity.Property(e => e.ProduktId).HasColumnName("ProduktID");

                entity.Property(e => e.ProduktNamn)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
