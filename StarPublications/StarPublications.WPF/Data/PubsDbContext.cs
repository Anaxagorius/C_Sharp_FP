using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StarPublications.Models;

namespace StarPublications.Data;

public class PubsDbContext : DbContext
{
    public DbSet<Title> Titles { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<TitleAuthor> TitleAuthors { get; set; }
    public DbSet<Store> Stores { get; set; }
    public DbSet<Sale> Sales { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("PubsDb");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TitleAuthor>()
            .HasKey(ta => new { ta.AuthorId, ta.TitleId });

        modelBuilder.Entity<Sale>()
            .HasKey(s => new { s.StoreId, s.OrderNumber, s.TitleId });

        modelBuilder.Entity<TitleAuthor>()
            .HasOne(ta => ta.Author)
            .WithMany(a => a.TitleAuthors)
            .HasForeignKey(ta => ta.AuthorId);

        modelBuilder.Entity<TitleAuthor>()
            .HasOne(ta => ta.Title)
            .WithMany(t => t.TitleAuthors)
            .HasForeignKey(ta => ta.TitleId);

        modelBuilder.Entity<Title>()
            .HasOne(t => t.Publisher)
            .WithMany(p => p.Titles)
            .HasForeignKey(t => t.PubId);

        modelBuilder.Entity<Sale>()
            .HasOne(s => s.Store)
            .WithMany(st => st.Sales)
            .HasForeignKey(s => s.StoreId);

        modelBuilder.Entity<Sale>()
            .HasOne(s => s.Title)
            .WithMany(t => t.Sales)
            .HasForeignKey(s => s.TitleId);
    }
}
