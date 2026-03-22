using System.Diagnostics;
using System.Text;

// Configuration
// const int RecordCount = 10_000_000;
const int RecordCount = 1_000_000;
const string FilePath = "large_transactions.csv";
const int BufferSize = 65536; // 64KB buffer for file writing

// Data sources
string[] transactionTypes = ["Credit", "Debit", "Transfer"];
string[] currencies = ["USD", "EUR", "GBP", "JPY", "AUD", "CAD", "CHF", "CNY", "SEK", "NZD"];
string[] cardTypes = ["Visa", "Mastercard", "Amex", "Discover", "Diners Club"];
string[] reversalIndicators = ["N", "N", "N", "N", "N", "N", "N", "N", "N", "Y"]; // 10% chance of reversal
string[] narratives = ["payment", "transfer", "refund", "fee", "tax", "salary", "rent", "bill", "subscription", "purchase"];
char[] alphaNumeric = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

// MCC Ranges
(int Min, int Max)[] mccRanges =
[
    (0001, 1499), // Agricultural services
    (1500, 2999), // Contracted services
    (4000, 4799), // Transportation services
    (4800, 4999), // Utility services
    (5000, 5599), // Retail outlet services
    (5600, 5699), // Clothing shops
    (5700, 7299), // Miscellaneous shops
    (7300, 7999), // Business services
    (8000, 8999), // Professional services and membership organisations
    (9000, 9999)  // Government services
];

Console.WriteLine($"Starting generation of {RecordCount:N0} records to {FilePath}...");
var sw = Stopwatch.StartNew();

using (var fileStream = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.None, BufferSize))
using (var writer = new StreamWriter(fileStream, new UTF8Encoding(false))) // UTF8 without BOM similar to original if needed, or standard
{
    // Write Header
    writer.WriteLine("TransactionId|AccountId|TransactionDate|TransactionType|TransactionCurrency|TransactionAmount|TransactionNarrative|ReversalIndicator|MID|CardType|MCC");

    var rnd = new Random();
    var sb = new StringBuilder(256); // Reusable string builder buffer if we were doing custom formatting

    // Pre-calculate date range
    var startDate = new DateTime(2025, 1, 1);
    var rangeDays = (DateTime.Today - startDate).Days;

    // Allocate buffer for MID once
    Span<char> midSpan = stackalloc char[15];

    for (int i = 0; i < RecordCount; i++)
    {
        // 1. TransactionId
        var transactionId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        // 2. Date
        var date = startDate.AddDays(rnd.Next(rangeDays));

        // 3. Select randoms
        var type = transactionTypes[rnd.Next(transactionTypes.Length)];
        var currency = currencies[rnd.Next(currencies.Length)];
        var amount = (rnd.NextDouble() * 10000).ToString("F2");
        var narrative = narratives[rnd.Next(narratives.Length)];
        var reversal = reversalIndicators[rnd.Next(reversalIndicators.Length)];
        var card = cardTypes[rnd.Next(cardTypes.Length)];

        // 4. MCC
        var range = mccRanges[rnd.Next(mccRanges.Length)];
        var mcc = rnd.Next(range.Min, range.Max + 1).ToString("D4");

        // 5. MID (Random String)
        for (int k = 0; k < 15; k++) midSpan[k] = alphaNumeric[rnd.Next(alphaNumeric.Length)];

        // Writing line
        writer.Write(transactionId);
        writer.Write('|');
        writer.Write(accountId);
        writer.Write('|');
        writer.Write(date.ToString("dd/MM/yyyy"));
        writer.Write('|');
        writer.Write(type);
        writer.Write('|');
        writer.Write(currency);
        writer.Write('|');
        writer.Write(amount);
        writer.Write('|');
        writer.Write(narrative);
        writer.Write('|');
        writer.Write(reversal);
        writer.Write('|');
        writer.Write(midSpan);
        writer.Write('|');
        writer.Write(card);
        writer.Write('|');
        writer.WriteLine(mcc);

        if (i % 500_000 == 0)
        {
            Console.WriteLine($"Generated {i:N0} records... {sw.Elapsed}");
        }
    }
}

sw.Stop();
Console.WriteLine($"Done! Generated {RecordCount:N0} records in {sw.Elapsed.TotalSeconds:F2} seconds.");
Console.WriteLine($"File size: {new FileInfo(FilePath).Length / 1024 / 1024} MB");

