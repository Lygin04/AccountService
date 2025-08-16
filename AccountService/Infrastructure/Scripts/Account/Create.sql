INSERT INTO accounts (id, owner_id, type, currency_code, balance, interest_rate, open_date, close_date)
VALUES (@Id,
        @OwnerId,
        @Type,
        @Currency,
        @Balance,
        @InterestRate,
        @OpenDate,
        @CloseDate);