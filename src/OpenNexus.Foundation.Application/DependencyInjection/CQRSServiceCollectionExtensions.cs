// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using OpenNexus.Foundation.CQRS;
// using OpenNexus.Foundation.Primitives;

// namespace OpenNexus.Foundation.Application;

// public static class CQRSServiceCollectionExtensions
// {
//     /// <summary>
//     /// Registers application services in the provided service collection.
//     /// </summary>
//     /// <param name="services">The service collection to add the services to</param>
//     /// <returns></returns>
//     public static IServiceCollection AddCQRSServices(this IServiceCollection services, IConfiguration configuration)
//     {
//         // Add the dispatcher service for handling commands and queries
//         services.AddScoped<IDispatcher, Dispatcher>();

//         // Get the assembly containing the ApplicationServiceCollectionExtensions class and retrieve all types in it
//         var assembly = typeof(CQRSServiceCollectionExtensions).Assembly;
//         var types = assembly.GetTypes();

//         // Scan the assembly for command and query handlers
//         services.AddCommandHandlers(types);
//         services.AddQueryHandlers(types);

//         // Add the pipeline behaviors for command and query handling
//         services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LogginPipelineBehavior<,>));

//         // Return the modified service collection
//         return services;
//     }

//     /// <summary>
//     /// Registers command handlers in the service collection by scanning the assembly for implementations of ICommandHandler.
//     /// </summary>
//     /// <param name="services"></param>
//     /// <returns></returns>
//     public static IServiceCollection AddCommandHandlers(this IServiceCollection services, IEnumerable<Type> types)
//     {
//         return services.AddHandlers(types, typeof(ICommandHandler<,>));
//     }

//     /// <summary>
//     /// Registers query handlers in the service collection by scanning the assembly for implementations of IQueryHandler.
//     /// </summary>
//     /// <param name="services"></param>
//     /// <returns></returns>
//     public static IServiceCollection AddQueryHandlers(this IServiceCollection services, IEnumerable<Type> types)
//     {
//         return services.AddHandlers(types, typeof(IQueryHandler<,>));
//     }

//     /// <summary>
//     /// Registers handlers in the service collection by scanning the assembly for implementations of a specified generic handler type.
//     /// </summary>
//     /// <param name="services"></param>
//     /// <param name="types"></param>
//     /// <param name="genericHandlerType"></param>
//     /// <returns></returns>
//     private static IServiceCollection AddHandlers(this IServiceCollection services, IEnumerable<Type> types, Type genericHandlerType)
//     {
//         // Validate the input parameters
//         Guard.AgainstNull(services, nameof(services));
//         Guard.AgainstNull(types, nameof(types));
//         Guard.AgainstNull(genericHandlerType, nameof(genericHandlerType));

//         // If the generic handler type is not a generic type definition, throw an exception
//         if (!genericHandlerType.IsGenericTypeDefinition)
//         {
//             throw new ArgumentException("The provided type must be a generic type definition.", nameof(genericHandlerType));
//         }

//         // Track already seen interface and implementation pairs to avoid duplicate registrations
//         var seen = new HashSet<(Type iface, Type impl)>();

//         // Get all the implementations of the ICommandHandler interface
//         var handlers = types.Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericHandlerType));

//         // Loop trough each handler type and register it
//         foreach (var impl in types)
//         {
//             // must be concrete, non-abstract, non-open-generic
//             if (impl is null || !impl.IsClass || impl.IsAbstract || impl.ContainsGenericParameters)
//                 continue;

//             // find all matching interfaces implemented by this type
//             var matches = impl.GetInterfaces()
//                 .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericHandlerType);

//             // Loop trough all the matching interfaces and register them
//             foreach (var iface in matches)
//             {
//                 // Check if the interface and implementation pair has already been seen
//                 if (seen.Add((iface, impl)))
//                 {
//                     // Register the interface and implementation in the service collection
//                     services.AddScoped(iface, impl);
//                 }
//             }
//         }

//         // Return the service collection for chaining
//         return services;
//     }
// }