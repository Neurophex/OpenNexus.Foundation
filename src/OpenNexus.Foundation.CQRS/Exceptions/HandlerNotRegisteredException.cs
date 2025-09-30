namespace OpenNexus.Foundation.CQRS;

/// <summary>
/// Represents an exception that is thrown when a handler for a specific request type is not registered.
/// </summary>
public sealed class HandlerNotRegisteredException : Exception
{
    public Type RequestType { get; }
    public Type HandlerInterfaceType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerNotRegisteredException"/> class with a specified request type and handler interface type.
    /// </summary>
    /// <param name="requestType"></param>
    /// <param name="handlerInterfaceType"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public HandlerNotRegisteredException(Type requestType, Type handlerInterfaceType) : 
        base(BuildMessage(requestType, handlerInterfaceType))
    {
        RequestType = requestType ?? throw new ArgumentNullException(nameof(requestType));
        HandlerInterfaceType = handlerInterfaceType ?? throw new ArgumentNullException(nameof(handlerInterfaceType));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerNotRegisteredException"/> class with a specified request type, handler interface type, and an inner exception.
    /// </summary>
    /// <param name="requestType"></param>
    /// <param name="handlerInterfaceType"></param>
    /// <param name="innerException"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public HandlerNotRegisteredException(Type requestType, Type handlerInterfaceType, Exception? innerException) : 
        base(BuildMessage(requestType, handlerInterfaceType), innerException)
    {
        RequestType = requestType ?? throw new ArgumentNullException(nameof(requestType));
        HandlerInterfaceType = handlerInterfaceType ?? throw new ArgumentNullException(nameof(handlerInterfaceType));
    }

    /// <summary>
    /// Builds the exception message for the handler not registered exception.
    /// </summary>
    /// <param name="requestType"></param>
    /// <param name="handlerInterfaceType"></param>
    /// <returns></returns>
    private static string BuildMessage(Type requestType, Type handlerInterfaceType) =>
        $"No handler is registered for request type '{requestType.FullName}'. " +
        $"Expected DI registration for '{handlerInterfaceType.FullName}'. " +
        "Ensure the handler is implemented and added to the container.";
}