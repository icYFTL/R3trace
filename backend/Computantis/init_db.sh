#!/bin/bash
set -e

psql -v ON_ERROR_STOP=1 --username "postgres" --dbname "postgres" <<-EOSQL
    CREATE USER computantis_slave WITH PASSWORD 'computantis';
    CREATE DATABASE retrace_computantis WITH OWNER 'computantis_slave';
EOSQL

psql -v ON_ERROR_STOP=1 --username "computantis_slave" --dbname "retrace_computantis" <<-EOSQL
    CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
EOSQL