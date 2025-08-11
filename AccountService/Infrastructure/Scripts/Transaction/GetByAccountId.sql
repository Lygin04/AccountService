SELECT id                       AS Id,
       account_id               AS AccountId,
       counterparty_account_id  AS CounterpartyAccountId,
       amount                   AS Amount,
       currency_code            AS Currency,
       type                     AS Type,
       description              AS Description,
       timestamp                AS Timestamp
FROM transactions
WHERE account_id = @AccountId;