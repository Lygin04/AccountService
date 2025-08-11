INSERT INTO transactions (account_id,
                          counterparty_account_id,
                          amount,
                          currency_code,
                          type,
                          description,
                          timestamp)
VALUES (@AccountId,
        @CounterpartyAccountId,
        @Amount,
        @Currency,
        @Type,
        @Description,
        @Timestamp);