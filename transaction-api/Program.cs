using Microsoft.EntityFrameworkCore;
using transaction_api.Data;
using transaction_api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add EF Core PostgreSQL DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Database=transactions_db;Username=postgres;Password=postgres";
builder.Services.AddDbContext<TransactionDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Map transaction endpoints
app.MapTransactionEndpoints();

app.Run();
