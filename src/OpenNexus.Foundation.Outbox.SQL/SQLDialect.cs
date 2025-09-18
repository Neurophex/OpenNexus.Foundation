namespace OpenNexus.Foundation.Outbox.SQL;

/// <summary>
/// Enumeration of supported SQL dialects for the outbox store.
/// </summary>
public enum SQLDialect
{
    /// <summary>
    /// Microsoft SQL Server
    /// </summary>
    SqlServer,

    /// <summary>
    /// PostgreSQL
    /// </summary>
    PostgreSQL,

    /// <summary>
    /// MySQL
    /// </summary>
    MySQL,

    /// <summary>
    /// SQLite
    /// </summary>
    SQLite
}

/// <summary>
/// Provides extension methods for the SQLDialect enum.
/// </summary>
public static class SQLDialectExtensions
{
    /// <summary>
    /// Convert the SQL dialect enum to a string representation.
    /// </summary>
    /// <param name="dialect"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public static string AsString(this SQLDialect dialect) => dialect switch
    {
        SQLDialect.SqlServer => "SqlServer",
        SQLDialect.PostgreSQL => "PostgreSQL",
        SQLDialect.MySQL => "MySQL",
        SQLDialect.SQLite => "SQLite",
        _ => throw new NotSupportedException($"The SQL dialect '{dialect}' is not supported.")
    };
}
