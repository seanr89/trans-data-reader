using System.Diagnostics;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Npgsql;
using NpgsqlTypes;

class Program
{
    static async Task Main(string[] args)
    {
        var stopwatch = Stopwatch.StartNew();
        string connectionString = "Host=localhost;Port=5432;Username=admin;Password=password;Database=transactions_db";
        string csvPath = "./large_transactions.csv";

        Console.WriteLine($"Starting ingestion from {csvPath} using CsvHelper...");

        try
        {
            await using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "|",
                HasHeaderRecord = true,
                MissingFieldFound = null
            };

            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, config);

            // Read header
            await csv.ReadAsync();
            csv.ReadHeader();

            // Batch size for committing commands to Postgres
            const int BatchSize = 500_000;
            int currentBatchCount = 0;

            // Function to start a new importer
            async Task<NpgsqlBinaryImporter> StartImporterAsync(NpgsqlConnection c)
            {
                return await c.BeginBinaryImportAsync("COPY transactions (TransactionId, AccountId, TransactionDate, TransactionType, TransactionCurrency, TransactionAmount, TransactionNarrative, ReversalIndicator, MID, CardType) FROM STDIN (FORMAT BINARY)");
            }

            var writer = await StartImporterAsync(conn);

            int rowCount = 0;
            while (await csv.ReadAsync())
            {
                await writer.StartRowAsync();

                // TransactionId (UUID)
                writer.Write(csv.GetField<Guid>(0), NpgsqlDbType.Uuid);

                // AccountId (UUID)
                writer.Write(csv.GetField<Guid>(1), NpgsqlDbType.Uuid);

                // TransactionDate (VARCHAR)
                writer.Write(csv.GetField<string>(2)!, NpgsqlDbType.Varchar);

                // TransactionType (VARCHAR)
                writer.Write(csv.GetField<string>(3)!, NpgsqlDbType.Varchar);

                // TransactionCurrency (CHAR(3))
                writer.Write(csv.GetField<string>(4)!, NpgsqlDbType.Char);

                // TransactionAmount (DECIMAL)
                writer.Write(csv.GetField<decimal>(5), NpgsqlDbType.Numeric);

                // TransactionNarrative (TEXT)
                writer.Write(csv.GetField<string>(6)!, NpgsqlDbType.Text);

                // ReversalIndicator (CHAR(1))
                writer.Write(csv.GetField<string>(7)!, NpgsqlDbType.Char);

                // MID (VARCHAR)
                writer.Write(csv.GetField<string>(8)!, NpgsqlDbType.Varchar);

                // CardType (VARCHAR)
                writer.Write(csv.GetField<string>(9)!, NpgsqlDbType.Varchar);

                // MCC (VARCHAR)
                writer.Write(csv.GetField<string>(10)!, NpgsqlDbType.Varchar);

                rowCount++;
                currentBatchCount++;

                if (currentBatchCount >= BatchSize)
                {
                    await writer.CompleteAsync();
                    await writer.DisposeAsync();
                    Console.WriteLine($"Committed batch of {BatchSize} rows. Total processed: {rowCount:N0}");
                    writer = await StartImporterAsync(conn);
                    currentBatchCount = 0;
                }
            }

            // Commit remaining
            if (currentBatchCount > 0)
            {
                await writer.CompleteAsync();
            }
            await writer.DisposeAsync();
            stopwatch.Stop();

            Console.WriteLine($"Import complete!");
            Console.WriteLine($"Processed {rowCount:N0} rows in {stopwatch.Elapsed.TotalSeconds:F2} seconds.");
            if (stopwatch.Elapsed.TotalSeconds > 0)
            {
                Console.WriteLine($"Throughput: {rowCount / stopwatch.Elapsed.TotalSeconds:N0} rows/sec");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}
