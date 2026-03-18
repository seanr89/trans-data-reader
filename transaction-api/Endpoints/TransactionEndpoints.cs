using Microsoft.EntityFrameworkCore;
using transaction_api.Data;
using transaction_api.Models;

namespace transaction_api.Endpoints;

public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/transactions").WithTags("Transactions");

        group.MapGet("/", async (TransactionDbContext db) =>
        {
            return await db.Transactions.Take(100).ToListAsync();
        });

        group.MapGet("/{id}", async (Guid id, TransactionDbContext db) =>
        {
            var transaction = await db.Transactions.FindAsync(id);
            return transaction is not null ? Results.Ok(transaction) : Results.NotFound();
        });
    }
}
