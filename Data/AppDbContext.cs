using Microsoft.EntityFrameworkCore;
using KovsieAssetTracker.Models;

namespace KovsieAssetTracker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Tables in your database
        public DbSet<Asset> Assets { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Verification> Verifications { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Asset
            modelBuilder.Entity<Asset>()
                .HasIndex(a => a.TagId)
                .IsUnique();

            modelBuilder.Entity<Verification>()
        .HasOne(v => v.Asset)
        .WithMany(a => a.Verifications)
        .HasForeignKey(v => v.AssetId)
        .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
