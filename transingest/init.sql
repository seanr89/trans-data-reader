CREATE TABLE IF NOT EXISTS transactions (
    TransactionId UUID PRIMARY KEY,
    AccountId UUID NOT NULL,
    TransactionDate VARCHAR(20), -- Format in CSV is dd/MM/yyyy
    TransactionType VARCHAR(50),
    TransactionCurrency CHAR(3),
    TransactionAmount DECIMAL(18, 2),
    TransactionNarrative TEXT,
    ReversalIndicator CHAR(1),
    MID VARCHAR(255),
    CardType VARCHAR(50)
);
