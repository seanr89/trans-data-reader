#!/bin/bash

# Create the initialization SQL script
echo "Creating init.sql..."
cat <<EOF > init.sql
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
    CardType VARCHAR(50),
    MCC VARCHAR(4)
);
EOF

# Create the Dockerfile
echo "Creating Dockerfile..."
cat <<EOF > Dockerfile
FROM postgres:latest

ENV POSTGRES_DB=transactions_db
ENV POSTGRES_USER=admin
ENV POSTGRES_PASSWORD=password

COPY init.sql /docker-entrypoint-initdb.d/
EOF

# Build the Docker image
echo "Building Docker image 'transactions-db-image'..."
docker build -t transactions-db-image .

echo "Build complete."
echo "To run the container, execute:"
echo "docker run -d --name transactions-db-container -p 5432:5432 transactions-db-image"
