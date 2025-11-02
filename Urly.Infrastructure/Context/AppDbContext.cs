using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urly.Domain.Entities;

namespace Urly.Infrastructure.Context;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UrlClick> UrlClickers { get; set; }
    public DbSet<ShortUrl> ShortUrls { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ShortUrl>()
                    .HasMany(c => c.Clicks)
                    .WithOne(s => s.ShortUrl)
                    .HasForeignKey(s => s.ShortUrlId);

        modelBuilder.Entity<ShortUrl>()
                    .HasIndex(s => s.ShortCode)
                    .IsUnique();
    }
}
