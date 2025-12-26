using Microsoft.EntityFrameworkCore;
using Domain.models;
namespace Infrastructure.Data;

public class ClinikTimeDbContext : DbContext
{
    public ClinikTimeDbContext(DbContextOptions<ClinikTimeDbContext> options)
        : base(options)
    {
    }

    // ======================
    // DbSets
    // ======================
    public DbSet<Utilisateur> Utilisateurs => Set<Utilisateur>();
    public DbSet<FichePatient> FichePatients => Set<FichePatient>();
    public DbSet<Medecin> Medecins => Set<Medecin>();
    public DbSet<Specialite> Specialites => Set<Specialite>();
    public DbSet<RendezVous> RendezVous => Set<RendezVous>();
    public DbSet<DisponibiliteMedecin> DisponibilitesMedecin => Set<DisponibiliteMedecin>();
    public DbSet<NoteMedicale> NotesMedicales => Set<NoteMedicale>();

    // ======================
    // Fluent API
    // ======================
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ======================
        // Utilisateur
        // ======================
        modelBuilder.Entity<Utilisateur>(builder =>
        {
            builder.ToTable("Utilisateur");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.HasIndex(u => u.Email)
                   .IsUnique();

            builder.Property(u => u.MotDePasseHash)
                   .IsRequired();

            builder.Property(u => u.Role)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(u => u.DateCreation)
                   .IsRequired();
        });

        // ======================
        // FichePatient
        // ======================
        modelBuilder.Entity<FichePatient>(builder =>
        {
            builder.ToTable("FichePatient");

            builder.HasKey(fp => fp.Id);

            builder.Property(fp => fp.Nom)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(fp => fp.Prenom)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(fp => fp.DateNaissance)
                   .IsRequired();

            builder.Property(fp => fp.Sexe)
                   .HasMaxLength(10);

            builder.Property(fp => fp.LienParente)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.HasOne(fp => fp.Utilisateur)
                   .WithMany(u => u.FichesPatients)
                   .HasForeignKey(fp => fp.UtilisateurId)
                   .OnDelete(DeleteBehavior.Cascade);
        });

        // ======================
        // Specialite
        // ======================
        modelBuilder.Entity<Specialite>(builder =>
        {
            builder.ToTable("Specialite");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Nom)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(s => s.Nom)
                   .IsUnique();

            builder.Property(s => s.DureeRdvMinutes)
                   .IsRequired();
        });

        // ======================
        // Medecin
        // ======================
        modelBuilder.Entity<Medecin>(builder =>
        {
            builder.ToTable("Medecin");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Nom)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(m => m.Prenom)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(m => m.Telephone)
                   .HasMaxLength(30);

            builder.HasOne(m => m.Utilisateur)
                   .WithOne(u => u.Medecin)
                   .HasForeignKey<Medecin>(m => m.UtilisateurId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Specialite)
                   .WithMany(s => s.Medecins)
                   .HasForeignKey(m => m.SpecialiteId)
                   .OnDelete(DeleteBehavior.Restrict);
        });

        // ======================
        // DisponibiliteMedecin
        // ======================
        modelBuilder.Entity<DisponibiliteMedecin>(builder =>
        {
            builder.ToTable("DisponibiliteMedecin");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Debut)
                   .IsRequired();

            builder.Property(d => d.Fin)
                   .IsRequired();

            builder.Property(d => d.EstBloquee)
                   .IsRequired();

            builder.HasOne(d => d.Medecin)
                   .WithMany(m => m.Disponibilites)
                   .HasForeignKey(d => d.MedecinId)
                   .OnDelete(DeleteBehavior.Cascade);
        });

        // ======================
        // RendezVous
        // ======================
        modelBuilder.Entity<RendezVous>(builder =>
        {
            builder.ToTable("RendezVous");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Debut)
                   .IsRequired();

            builder.Property(r => r.Fin)
                   .IsRequired();

            builder.Property(r => r.Motif)
                   .HasMaxLength(255);

            builder.Property(r => r.Statut)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.HasOne(r => r.Medecin)
                   .WithMany(m => m.RendezVous)
                   .HasForeignKey(r => r.MedecinId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.FichePatient) 
                   .WithMany(fp => fp.RendezVous)
                   .HasForeignKey(r => r.FichePatientId)
                   .OnDelete(DeleteBehavior.Cascade);

        });

        // ======================
        // NoteMedicale
        // ======================
        modelBuilder.Entity<NoteMedicale>(builder =>
        {
            builder.ToTable("NoteMedicale");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Date)
                   .IsRequired();

            builder.Property(n => n.Contenu)
                   .IsRequired();

            builder.HasOne<Medecin>()
                   .WithMany()
                   .HasForeignKey(n => n.MedecinId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<FichePatient>()
                   .WithMany()
                   .HasForeignKey(n => n.FichePatientId)
                   .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
