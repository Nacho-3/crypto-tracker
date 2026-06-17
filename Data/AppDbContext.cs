using Microsoft.EntityFrameworkCore;
using CryptoTrackerAPI.Models;

namespace CryptoTrackerAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Transaction> Transactions { get; set; } //representa la tabla
    }
}