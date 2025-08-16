SELECT id               AS Id,
       occurred_at      AS OccurredAt,
       type             AS Type,
       routing_key      AS RoutingKey,
       payload::text    AS PayloadJson,
        headers::text   AS HeadersJson,
        status          AS Status,
        attempts        AS Attempts,
        next_attempt_at AS NextAttemptAt,
        published_at    AS PublishedAt
FROM outbox_messages
WHERE status = 0
  AND next_attempt_at <= now()
ORDER BY occurred_at
    limit @batch