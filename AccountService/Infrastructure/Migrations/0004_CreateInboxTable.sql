create table if not exists inbox_consumed (
    message_id   uuid primary key,
    processed_at timestamptz not null default now(),
    handler      text        not null
);

create table if not exists inbox_dead_letters (
    message_id   uuid primary key,
    received_at  timestamptz not null default now(),
    handler      text        not null,
    payload      jsonb       not null,
    error        text        not null
);

create table if not exists audit_events (
    message_id   uuid primary key,
    received_at  timestamptz not null default now(),
    type         int        not null,
    routing_key  text        not null,
    payload      jsonb       not null
);