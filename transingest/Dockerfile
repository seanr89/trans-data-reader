FROM postgres:latest

ENV POSTGRES_DB=transactions_db
ENV POSTGRES_USER=admin
ENV POSTGRES_PASSWORD=password

COPY init.sql /docker-entrypoint-initdb.d/
