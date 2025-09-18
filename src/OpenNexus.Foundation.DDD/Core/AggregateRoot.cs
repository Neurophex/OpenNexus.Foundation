namespace OpenNexus.Foundation.DDD.Core;

/// <summary>
/// Base class for aggregate roots in the domain.
/// </summary>
/// <typeparam name="TIdentity">The type of Id for the given AggregateRoot</typeparam>
public abstract class AggregateRoot<TIdentity> : Entity<TIdentity>
{

    /// <summary>
    /// Default constructor for the <see cref="AggregateRoot{TIdentity}"/> class.
    /// </summary>
    protected AggregateRoot()
    {
        // This constructor is used by EF Core for materialization
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot{TIdentity}"/> class with a specified identity.
    /// </summary>
    /// <param name="id"></param>
    public AggregateRoot(TIdentity id) : base(id)
    {

    }
}
