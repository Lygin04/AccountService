SELECT id               AS Id,
       owner_id         AS OwnerId,
       type             AS Type,
       currency_code    AS Currency,
       balance          AS Balance,
       interest_rate    AS InterestRate,
       open_date        AS OpenDate,
       close_date       AS CloseDate,
       is_blocked       AS IsBlocked,
       xmin             AS Xmin
FROM accounts
WHERE id = @Id;