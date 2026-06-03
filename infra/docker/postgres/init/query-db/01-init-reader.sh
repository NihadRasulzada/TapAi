#!/bin/bash
# =====================================================
# TapAi - Query DB Init Script
# Docker initdb zamanı işləyir.
# tapai_reader — yalnız SELECT icazəsi olan runtime user.
# App bu user ilə qoşulur; admin user (tapai) yalnız
# EF Core migrations üçün istifadə olunur.
# =====================================================

set -e

READER_PASS="${QUERY_READER_PASSWORD:-tapai_reader_password}"
DB_NAME="${QUERY_DB_NAME:-tapai_query}"
ADMIN_USER="${QUERY_DB_USER:-tapai}"

echo "🔧 tapai_reader rolu yaradılır..."

psql -v ON_ERROR_STOP=1 -U "$ADMIN_USER" -d "$DB_NAME" <<-SQL
    DO \$\$
    BEGIN
      IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = 'tapai_reader') THEN
        CREATE ROLE tapai_reader
          WITH LOGIN
               PASSWORD '${READER_PASS}';
        RAISE NOTICE 'tapai_reader rolu yaradıldı.';
      ELSE
        ALTER ROLE tapai_reader WITH PASSWORD '${READER_PASS}';
        RAISE NOTICE 'tapai_reader rolu artıq mövcuddur, şifrə yeniləndi.';
      END IF;
    END
    \$\$;

    -- Qoşulma icazəsi
    GRANT CONNECT ON DATABASE "${DB_NAME}" TO tapai_reader;

    -- Schema istifadə icazəsi
    GRANT USAGE ON SCHEMA public TO tapai_reader;

    -- Mövcud cədvəllərə SELECT icazəsi
    GRANT SELECT ON ALL TABLES IN SCHEMA public TO tapai_reader;

    -- Gələcəkdə yaradılacaq cədvəllərə də avtomatik SELECT icazəsi
    ALTER DEFAULT PRIVILEGES IN SCHEMA public
        GRANT SELECT ON TABLES TO tapai_reader;
SQL

echo "✅ tapai_reader hazırdır."
echo "   Role: tapai_reader (SELECT-only)"
echo "   Database: ${DB_NAME}"
