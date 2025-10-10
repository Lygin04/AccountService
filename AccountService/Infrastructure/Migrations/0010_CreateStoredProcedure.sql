CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE OR REPLACE PROCEDURE accrue_interest(p_account_id UUID)
LANGUAGE plpgsql
AS $$
DECLARE
acc RECORD;
    daily_interest NUMERIC(18, 2);
BEGIN
    -- Получаем данные счета (только депозиты)
SELECT *
INTO acc
FROM accounts
WHERE id = p_account_id
  AND (type = 1 OR type = 2)
  AND close_date IS NULL;

-- Если счет не найден или не подходит — ничего не делаем
IF NOT FOUND THEN
        RAISE NOTICE 'Account not found or not eligible for interest accrual.';
        RETURN;
END IF;

    -- Если процентная ставка отсутствует — не начисляем
    IF acc.interest_rate IS NULL THEN
        RAISE NOTICE 'No interest rate set for account %', p_account_id;
        RETURN;
END IF;

    -- Расчёт процентов за один день
    daily_interest := ROUND(acc.balance * (acc.interest_rate / 100.0 / 365), 2);

    -- Если сумма нулевая — пропускаем
    IF daily_interest = 0 THEN
        RAISE NOTICE 'Zero daily interest for account %', p_account_id;
        RETURN;
END IF;

    -- Создаём транзакцию начисления процентов
INSERT INTO transactions (
    id,
    account_id,
    counterparty_account_id,
    amount,
    currency_code,
    type,
    description,
    timestamp
)
VALUES (
           uuid_generate_v4(),
           acc.id,
           acc.id, -- сам себе контрагент
           daily_interest,
           acc.currency_code,
           0,
           'Daily interest accrued',
           now()
       );

-- Обновляем баланс счета
UPDATE accounts
SET balance = balance + daily_interest
WHERE id = acc.id;

RAISE NOTICE 'Daily interest % accrued to account %', daily_interest, acc.id;
END;
$$;