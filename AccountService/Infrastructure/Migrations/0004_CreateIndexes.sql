CREATE INDEX accounts_owner_id_hash_idx
    ON accounts USING hash (owner_id);

CREATE INDEX transactions_account_id_timestamp_idx
    ON transactions (account_id, timestamp);

CREATE EXTENSION IF NOT EXISTS btree_gist;
CREATE INDEX transactions_timestamp_gist_idx
    ON transactions USING gist (timestamp);