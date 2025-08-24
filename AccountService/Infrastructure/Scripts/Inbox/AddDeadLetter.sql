INSERT INTO inbox_dead_letters(message_id, handler, payload, error)
VALUES (@MessageId, @Handler, cast(@PayloadJson AS jsonb), @error)
ON conflict do nothing