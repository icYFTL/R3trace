#!/bin/bash
set -e

psql -v ON_ERROR_STOP=1 --username "postgres" --dbname "postgres" <<-EOSQL
    CREATE USER vexillum_slave WITH PASSWORD 'vexillum';
    CREATE DATABASE retrace_vexillum WITH OWNER 'vexillum_slave';
EOSQL

psql -v ON_ERROR_STOP=1 --username "vexillum_slave" --dbname "retrace_vexillum" <<-EOSQL
    CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
EOSQL