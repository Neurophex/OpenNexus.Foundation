using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OpenNexus.Foundation.Application;

/// <summary>
/// CQRS service initializer.
/// </summary>
public sealed class CQRSServiceInitializer : IServiceInitializer
{
    public async Task Initialize(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        // Discover and log the registered CQRS handlers
        var logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger("CQRSServiceInitializer");
    }
}