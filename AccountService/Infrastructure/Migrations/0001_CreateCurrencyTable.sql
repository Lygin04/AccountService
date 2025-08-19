CREATE TABLE currencies (
    code INTEGER PRIMARY KEY,
    symbol CHAR(3) NOT NULL UNIQUE
);

INSERT INTO currencies (code, symbol) VALUES
    (643, 'RUB'),
    (840, 'USD'),
    (978, 'EUR');