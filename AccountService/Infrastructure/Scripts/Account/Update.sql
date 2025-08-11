UPDATE accounts
SET balance         = @Balance,
    interest_rate   = @InterestRate
WHERE id = @Id;