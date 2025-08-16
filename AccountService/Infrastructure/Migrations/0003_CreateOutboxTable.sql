create table outbox_messages (
    id               uuid primary key,
    occurred_at      timestamptz not null,
    type             text        not null,
    routing_key      text        not null,
    payload          jsonb       not null,
    headers          jsonb       not null,
    status           int        not null,
    attempts         int         not null default 0,
    next_attempt_at  timestamptz not null default now(),
    published_at     timestamptz null
);

create index if not exists ix_outbox_status_nextattempt on outbox_messages (status, next_attempt_at);
create index if not exists ix_outbox_occurred_at on outbox_messages (occurred_at);