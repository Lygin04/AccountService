UPDATE outbox_messages
SET status = 1, published_at=now()
WHERE id=@Id