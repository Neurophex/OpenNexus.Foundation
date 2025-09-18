using System.Reflection;

namespace OpenNexus.Foundation.Outbox.SQL;

/// <summary>
/// Options for configuring the outbox table schema and name.
/// </summary>
public sealed record OutboxTableOptions
{
    /// <summary>
    /// The schema where the outbox table resides.
    /// </summary>
    public string Schema { get; init; } = "public";

    /// <summary>
    /// The name of the outbox table.
    /// </summary>
    public string TableName { get; init; } = "outbox_messages";
}

/// <summary>
/// Provides SQL scripts for creating the outbox table for different SQL dialects.
/// </summary>
public static class SQLOutboxSchema
{
    /// <summary>
    /// Get the SQL script to create the outbox table for the specified SQL dialect.
    /// </summary>
    /// <param name="dialect"></param>
    /// <param name="options"></param>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static string GetCreateScript(SQLDialect dialect, OutboxTableOptions? options, Assembly? assembly)
    {
        // Get the SQL script for creating the outbox table
        var sql = GetSQLScript(SQLOutboxAction.CreateTable, dialect, options, assembly);

        // Return the final SQL script
        return sql;
    }

    /// <summary>
    /// Get the SQL script to drop the outbox table for the specified SQL dialect.
    /// </summary>
    /// <param name="dialect"></param>
    /// <param name="options"></param>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static string GetDropScript(SQLDialect dialect, OutboxTableOptions? options, Assembly? assembly)
    {
        // Get the SQL script for dropping the outbox table
        var sql = GetSQLScript(SQLOutboxAction.DropTable, dialect, options, assembly);

        // Return the final SQL script
        return sql;
    }

    /// <summary>
    /// Get the SQL script for the specified outbox action and SQL dialect.
    /// </summary>
    /// <param name="action"></param>
    /// <param name="dialect"></param>
    /// <param name="options"></param>
    /// <param name="assembly"></param>
    /// <returns></returns>
    internal static string GetSQLScriptForAction(SQLOutboxAction action, SQLDialect dialect, OutboxTableOptions? options, Assembly? assembly = null)
    {
        return GetSQLScript(action, dialect, options, assembly);
    }

    /// <summary>
    /// Helper method to load the appropriate SQL script based on the SQL dialect.
    /// </summary>
    /// <param name="scriptName"></param>
    /// <param name="sqlDialect"></param>
    /// <param name="options"></param>
    /// <param name="assembly"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    private static string GetSQLScript(SQLOutboxAction action, SQLDialect sqlDialect, OutboxTableOptions? options, Assembly? assembly)
    {
        // Determine which assembly to use for resource loading
        var usingAssembly = assembly ?? Assembly.GetExecutingAssembly();

        // Convert the SQL dialect enum and action to a string
        var dialectString = sqlDialect.AsString();
        var actionString = action.AsString();

        // Determine the resource name based on the SQL dialect and the name of the script
        var resourceName = $"OpenNexus.Common.Outbox.SQL.Scripts.{actionString}.{dialectString}.sql";

        // Get the embedded resource content
        var sql = GetEmbeddedResource(resourceName, usingAssembly, sqlDialect, action);

        // Use default options if none are provided
        var outboxOptions = options ?? new OutboxTableOptions();

        // Apply the tokens
        sql = ApplyTokens(sql, outboxOptions);

        // Return the SQL script
        return sql;
    }

    /// <summary>
    /// Helper method to replace tokens in the SQL script with actual values from options.
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    private static string ApplyTokens(string sql, OutboxTableOptions options)
    {
        // Validate options table name
        if (string.IsNullOrWhiteSpace(options.TableName))
            throw new ArgumentException("TableName cannot be null or empty.", nameof(options.TableName));
        
        // Validate options schema
        if (string.IsNullOrWhiteSpace(options.Schema))
            throw new ArgumentException("Schema cannot be null or empty.", nameof(options.Schema));

        // Replace tokens in the SQL script
        sql = sql.Replace("{{OutboxTableName}}", options.TableName, StringComparison.OrdinalIgnoreCase);
        sql = sql.Replace("{{OutboxSchema}}", options.Schema, StringComparison.OrdinalIgnoreCase);

        // Return the modified SQL script
        return sql;
    }

    /// <summary>
    /// Helper method to read an embedded resource from the specified assembly.
    /// </summary>
    /// <param name="resourceName"></param>
    /// <param name="assembly"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static string GetEmbeddedResource(string resourceName, Assembly assembly, SQLDialect sqlDialect, SQLOutboxAction action)
    {
        // Get the manifest resource stream
        using var stream = assembly.GetManifestResourceStream(resourceName);

        // Ensure the stream is not null
        if (stream == null)
        {
            throw new InvalidOperationException(
                $"Outbox SQL script '{resourceName}' not found (Dialect={sqlDialect.AsString()}, Action={action.AsString()}). " +
                $"Did you forget to embed the SQL file?");
        }

        // Read the stream content
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
