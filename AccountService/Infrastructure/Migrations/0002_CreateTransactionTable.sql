CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE transactions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    account_id UUID NOT NULL REFERENCES accounts(id) ON DELETE CASCADE,
    counterparty_account_id UUID NOT NULL,
    amount NUMERIC(18, 2) NOT NULL,
    currency_code INTEGER NOT NULL,
    type INTEGER NOT NULL,
    description TEXT NOT NULL DEFAULT '',
    timestamp TIMESTAMP NOT NULL
);

CREATE INDEX transactions_account_id_timestamp_idx
    ON transactions (account_id, timestamp);

CREATE EXTENSION IF NOT EXISTS btree_gist;
CREATE INDEX transactions_timestamp_gist_idx
    ON transactions USING gist (timestamp);