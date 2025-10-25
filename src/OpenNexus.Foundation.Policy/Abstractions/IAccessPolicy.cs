using OpenNexus.Foundation.Policy.Access;

namespace OpenNexus.Foundation.Policy.Abstractions;

/// <summary>
/// Defines access policies for a specific resource type.
/// </summary>
/// <typeparam name="TResource"></typeparam>
public interface IAccessPolicy<TResource>
{
    /// <summary>
    /// Determines if the user can create a new resource.
    /// </summary>
    /// <remarks>
    /// This check is typically performed before a specific instance is known. 
    /// Implementations that require contextual checks should also override <see cref="CanCreateResource(IUserContext, TResource)"/>.
    /// </remarks>
    /// <param name="user"></param>
    /// <returns></returns>
    IAccessDecision CanCreateResource(IUserContext user) => AccessDecision.Deny("Creation is not allowed by default.");

    /// <summary>
    /// Determines if the user can create the specified resource.
    /// </summary>
    /// <remarks>
    /// This is typically invoked after the resource instance has been constructed but not yet persisted.
    /// </remarks>
    /// <param name="user"></param>
    /// <param name="resource"></param>
    /// <returns></returns>
    IAccessDecision CanCreateResource(IUserContext user, TResource resource) => CanCreateResource(user);

    /// <summary>
    /// Determines if the user can view the specified resource.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="resource"></param>
    /// <returns></returns>
    IAccessDecision CanViewResource(IUserContext user, TResource resource);

    /// <summary>
    /// Determines if the user can edit the specified resource.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="resource"></param>
    /// <returns></returns>
    IAccessDecision CanEditResource(IUserContext user, TResource resource) => AccessDecision.Deny("Editing is not allowed by default.");

    /// <summary>
    /// Determines if the user can delete the specified resource.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="resource"></param>
    /// <returns></returns>
    IAccessDecision CanDeleteResource(IUserContext user, TResource resource) => AccessDecision.Deny("Deletion is not allowed by default.");
}
