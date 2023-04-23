#!/bin/bash
set -e

psql -v ON_ERROR_STOP=1 --username "postgres" --dbname "postgres" <<-EOSQL
    CREATE USER custos_slave WITH PASSWORD 'custos';
    CREATE DATABASE retrace_custos WITH OWNER 'custos_slave';
EOSQL

psql -v ON_ERROR_STOP=1 --username "custos_slave" --dbname "retrace_custos" <<-EOSQL
    CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
EOSQL