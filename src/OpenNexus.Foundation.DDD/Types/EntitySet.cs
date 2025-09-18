using OpenNexus.Foundation.DDD.Core;
using OpenNexus.Foundation.DDD.Validators;
using OpenNexus.Foundation.Utils;

namespace OpenNexus.Foundation.DDD.Types;

/// <summary>
/// A collection of entities
/// </summary>
/// <typeparam name="T">The type of the Entity stored within this set</typeparam>
/// <typeparam name="TIdentity">The type of the Entity id</typeparam>
public sealed class EntitySet<T, TIdentity> : DomainSetBase<T>
    where T : Entity<TIdentity>
    where TIdentity : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EntitySet{T, TIdentity}"/> class.
    /// </summary>
    public EntitySet() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntitySet{T, TIdentity}"/> class.
    /// </summary>
    /// <param name="validators"></param>
    public EntitySet(params ISetValidator<T>[] validators) : base(validators) { }

    /// <summary>
    /// Finds an entity by its id
    /// </summary>
    /// <param name="id">The id of the entity to look for</param>
    /// <returns></returns>
    public Result<T> FindById(TIdentity id) => Find(e => EqualityComparer<TIdentity>.Default.Equals(e.Id, id));

    /// <summary>
    /// Checks if an entity with the given id exists in the set
    /// </summary>
    /// <param name="id">The id of the entity to look for</param>
    /// <returns></returns>
    public bool ContainsId(TIdentity id) => _items.Any(e => EqualityComparer<TIdentity>.Default.Equals(e.Id, id));
    
    /// <summary>
    /// Creates a new instance of the <see cref="EntitySet{T, TIdentity}"/> class with validation.
    /// Validates each item using the provided set validators before adding them to the set.
    /// If any item fails validation, an error result is returned.
    /// </summary>
    /// <param name="items"></param>
    /// <param name="setValidators"></param>
    /// <returns></returns>
    public static Result<EntitySet<T, TIdentity>> CreateValidated(IEnumerable<T> items, params ISetValidator<T>[] setValidators)
    {
        return CreateValidated<EntitySet<T, TIdentity>>(items, setValidators);
    }
}
