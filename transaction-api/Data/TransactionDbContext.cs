using Microsoft.EntityFrameworkCore;
using transaction_api.Models;

namespace transaction_api.Data;

public class TransactionDbContext : DbContext
{
    public TransactionDbContext(DbContextOptions<TransactionDbContext> options)
        : base(options)
    {
    }

    public DbSet<Transaction> Transactions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Map to table and set primary key
        modelBuilder.Entity<Transaction>().ToTable("Transactions");
        modelBuilder.Entity<Transaction>().HasKey(t => t.TransactionId);
    }
}
