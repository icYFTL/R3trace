#!/bin/bash
set -e

psql -v ON_ERROR_STOP=1 --username "postgres" --dbname "postgres" <<-EOSQL
    CREATE USER magister_slave WITH PASSWORD 'magister';
    CREATE DATABASE retrace_magister WITH OWNER 'magister_slave';
EOSQL

psql -v ON_ERROR_STOP=1 --username "magister_slave" --dbname "retrace_magister" <<-EOSQL
    CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
EOSQL