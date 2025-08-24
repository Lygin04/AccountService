CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE accounts (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    owner_id UUID NOT NULL,
    type INTEGER NOT NULL,
    currency_code INTEGER NOT NULL,
    balance NUMERIC(18, 2) NOT NULL DEFAULT 0.00,
    interest_rate NUMERIC(5, 2),
    is_blocked BOOLEAN NOT NULL DEFAULT false,
    open_date TIMESTAMP NOT NULL,
    close_date TIMESTAMP
);

CREATE INDEX accounts_owner_id_hash_idx
    ON accounts USING hash (owner_id);