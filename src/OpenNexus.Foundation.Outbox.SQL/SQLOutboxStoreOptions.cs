namespace OpenNexus.Foundation.Outbox.SQL;

/// <summary>
/// Configuration options for the SQL-based outbox store.
/// </summary>
public sealed class SQLOutboxStoreOptions
{
    /// <summary>
    /// The SQL dialect to use (e.g., SQL Server, PostgreSQL, MySQL, SQLite).
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// The SQL dialect to use (e.g., SQL Server, PostgreSQL, MySQL, SQLite).
    /// </summary>
    public SQLDialect Dialect { get; set; } = SQLDialect.PostgreSQL;
}
