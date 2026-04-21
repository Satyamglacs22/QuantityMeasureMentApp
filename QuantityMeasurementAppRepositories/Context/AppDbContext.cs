using Microsoft.EntityFrameworkCore;
using QuantityMeasurementAppModels.Entities;

namespace QuantityMeasurementAppRepositories.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<QuantityMeasurementEntity> QuantityMeasurements { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ==================== QuantityMeasurementEntity ====================
            modelBuilder.Entity<QuantityMeasurementEntity>(entity =>
            {
                entity.ToTable("quantity_measurements");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Operation).HasColumnName("operation").IsRequired().HasMaxLength(50);
                entity.Property(e => e.FirstValue).HasColumnName("first_value");
                entity.Property(e => e.FirstUnit).HasColumnName("first_unit").HasMaxLength(50);
                entity.Property(e => e.SecondValue).HasColumnName("second_value");
                entity.Property(e => e.SecondUnit).HasColumnName("second_unit").HasMaxLength(50);
                entity.Property(e => e.ResultValue).HasColumnName("result_value");
                entity.Property(e => e.MeasurementType).HasColumnName("measurement_type").HasMaxLength(50);
                entity.Property(e => e.IsError).HasColumnName("is_error");
                entity.Property(e => e.ErrorMessage).HasColumnName("error_message").HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
                entity.HasIndex(e => e.Operation).HasDatabaseName("IX_quantity_measurements_operation");
                entity.HasIndex(e => e.MeasurementType).HasDatabaseName("IX_quantity_measurements_measurement_type");
                entity.HasIndex(e => e.IsError).HasDatabaseName("IX_quantity_measurements_is_error");
            });

            // ==================== UserEntity ====================
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.Role).HasMaxLength(50).HasDefaultValue("User");
                entity.Property(e => e.FailedLoginAttempts).IsRequired().HasDefaultValue(0);
                entity.HasMany(e => e.RefreshTokens)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ==================== RefreshTokenEntity ====================
            modelBuilder.Entity<RefreshTokenEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Token).IsRequired().HasMaxLength(200);
                entity.HasIndex(e => e.Token).IsUnique();
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.ExpiresAt).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.RevokedByIp).HasMaxLength(50);
                entity.Property(e => e.ReplacedByToken).HasMaxLength(200);
                entity.Property(e => e.CreatedByIp).HasMaxLength(50);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.ExpiresAt);
            });
        }
    }
}