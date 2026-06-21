CREATE EXTENSION IF NOT EXISTS timescaledb;

CREATE TABLE IF NOT EXISTS machines.system_snapshots (
    time               TIMESTAMPTZ     NOT NULL,
    machine_id         UUID            NOT NULL,
    cpu_usage_pct      NUMERIC(5,2),
    ram_used_gb        NUMERIC(5,1),
    disk_used_gb       NUMERIC(8,1),
    disk_free_pct      NUMERIC(5,2),
    network_in_mbps    NUMERIC(8,2),
    network_out_mbps   NUMERIC(8,2),
    top_processes_json JSONB,
    event_log_errors   INTEGER,
    hardware_json      JSONB,
    software_list_json JSONB
);

SELECT create_hypertable(
    'machines.system_snapshots',
    by_range('time', INTERVAL '7 days'),
    if_not_exists => TRUE
);

SELECT add_retention_policy(
    'machines.system_snapshots',
    INTERVAL '90 days',
    if_not_exists => TRUE
);
