INSERT INTO inbox_consumed(message_id, handler) VALUES (@MessageId, @Handler)
    ON conflict do nothing