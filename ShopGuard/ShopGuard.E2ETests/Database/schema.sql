-- Schema of the order validation database.
-- SQLite dialect; kept ANSI-compatible so it also runs on SQL Server
-- (replace AUTOINCREMENT with IDENTITY when migrating).
CREATE TABLE IF NOT EXISTS Orders (
    Id            INTEGER PRIMARY KEY AUTOINCREMENT,
    OrderNumber   TEXT    NOT NULL UNIQUE,
    CustomerEmail TEXT    NOT NULL,
    Total         NUMERIC NOT NULL,
    Status        TEXT    NOT NULL,
    CreatedAtUtc  TEXT    NOT NULL
);
