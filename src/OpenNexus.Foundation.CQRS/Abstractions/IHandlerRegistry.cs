namespace OpenNexus.Foundation.CQRS;

/// <summary>
/// Registry for command and query handlers in the CQRS pattern.
/// </summary>
public interface IHandlerRegistry
{
    /// <summary>
    /// Gets the total number of registered handlers.
    /// </summary>
    /// <returns></returns>
    // int GetRegisteredHandlerCount();

    /// <summary>
    /// Gets the total number of registered command handlers.
    /// </summary>
    /// <returns></returns>
    // int GetRegisteredCommandHandlerCount();

    /// <summary>
    /// Gets the total number of registered query handlers.
    /// </summary>
    /// <returns></returns>
    // int GetRegisteredQueryHandlerCount();

    /// <summary>
    /// Gets the list of registered handlers.
    /// </summary>
    /// <returns></returns>
    // IEnumerable<string> GetRegisteredHandlers();

    /// <summary>
    /// Gets the list of registered command handlers.
    /// </summary>
    /// <returns></returns>
    // IEnumerable<string> GetRegisteredCommandHandlers();

    /// <summary>
    /// Gets the list of registered query handlers.
    /// </summary>
    /// <returns></returns>
    // IEnumerable<string> GetRegisteredQueryHandlers();

    /// <summary>
    /// Creates an instance of the command handler for the specified command type.
    /// </summary>
    /// <param name="commandType"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    object CreateCommandHandler(Type commandType, IServiceProvider provider);

    /// <summary>
    /// Creates an instance of the query handler for the specified query type.
    /// </summary>
    /// <param name="commandType"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    object CreateQueryHandler(Type commandType, IServiceProvider provider);
}