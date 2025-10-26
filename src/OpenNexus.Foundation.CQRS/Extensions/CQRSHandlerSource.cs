using System.Reflection;

namespace OpenNexus.Foundation.CQRS.Extensions;

/// <summary>
/// CQRS handler assembly record.
/// Simple record to encapsulate an assembly containing CQRS handlers.
/// </summary>
/// <param name="Assembly"></param>
public record struct CQRSHandlerAssembly(Assembly Assembly);