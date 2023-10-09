using Microsoft.EntityFrameworkCore;

namespace FirstApi.Entities;

public partial class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options){ }
    public virtual DbSet<UserEntity> Users { get; set; }
    public virtual DbSet<WorkEntity> Works { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }

    // Fluent API: qui si definiscono le relazioni tra le tabelle
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("user");
            entity.Property(e => e.WorkId).HasColumnType("int(11)");
            entity.Property(e => e.Gender).HasColumnType("int(11)");
            entity.Property(e => e.EnrollmentDate).HasColumnType("datetime");
            entity.Property(e => e.FirstName).HasMaxLength(45);
            entity.Property(e => e.LastName).HasMaxLength(45);
            entity.Property(e => e.Password).HasMaxLength(45);
            entity.Property(e => e.UserName).HasMaxLength(45);

            // Definizione della chiave esterna
            entity.HasOne(e => e.Work)
                .WithMany()
                .HasForeignKey(e => e.WorkId)
                .IsRequired(); // Specifica se la chiave esterna è obbligatoria
        });

        modelBuilder.Entity<WorkEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("work");
            entity.Property(e => e.Name).HasMaxLength(45);
            entity.Property(e => e.Company).HasMaxLength(45);
            entity.Property(e => e.Salary).HasMaxLength(45);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}