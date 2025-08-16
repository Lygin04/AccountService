UPDATE outbox_messages
SET status = 0, attempts=attempts + 1, next_attempt_at = @NextAttemptAt
WHERE id = @Id