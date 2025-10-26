namespace OpenNexus.Foundation.Application;

/// <summary>
/// Interface for application initializers.
/// </summary>
public interface IServiceInitializer
{
    /// <summary>
    /// Initializes the application using the provided service provider.
    /// </summary>
    /// <param name="serviceProvider"></param>
    Task Initialize(IServiceProvider serviceProvider, CancellationToken cancellationToken = default);
}
