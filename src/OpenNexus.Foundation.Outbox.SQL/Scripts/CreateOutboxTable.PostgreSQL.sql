CREATE TABLE IF NOT EXISTS {{OutboxSchema}}.{{OutboxTableName}} (
    id uuid PRIMARY KEY,
    occurredon timestamptz NOT NULL,
    type text NOT NULL,
    payload text NOT NULL,
    status text NOT NULL,
    correlationid text NULL,
    causationid text NULL,
    attemptcount int NOT NULL DEFAULT 0,
    processedon timestamptz NULL,
    error text NULL,
    leaseuntil timestamptz NULL
);

CREATE INDEX IF NOT EXISTS ix_{{OutboxTableName}}_status_lease
    ON {{OutboxSchema}}.{{OutboxTableName}}(status, leaseuntil);