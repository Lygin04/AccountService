CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE accounts (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    owner_id UUID NOT NULL,
    type INTEGER NOT NULL,
    currency_code INTEGER NOT NULL REFERENCES currencies(code),
    balance NUMERIC(18, 2) NOT NULL DEFAULT 0.00,
    interest_rate NUMERIC(5, 2),
    open_date TIMESTAMP NOT NULL,
    close_date TIMESTAMP
);