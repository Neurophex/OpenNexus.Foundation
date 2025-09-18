using System.Collections;
using System.Collections.Immutable;
using OpenNexus.Foundation.DDD.Validators;
using OpenNexus.Foundation.Utils;

namespace OpenNexus.Foundation.DDD.Types;

/// <summary>
/// An abstract base class for a collection of domain objects.
/// It provides methods to add, remove, and query items in the collection.
/// Items stored in the collection are stored in a HashSet to ensure uniqueness.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class DomainSetBase<T> : IReadOnlyCollection<T>
    where T : class
{
    // The items of the collection
    protected readonly HashSet<T> _items;

    // The validators for adding items to the collection
    private readonly List<ISetValidator<T>> _setValidators;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainSet{T}"/> class.
    /// Calls the main constructor with an empty array of validators.
    /// </summary>
    public DomainSetBase() : this([]) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainSet{T}"/> class.
    /// </summary>
    public DomainSetBase(params ISetValidator<T>[] setValidators)
    {
        _items = [];
        _setValidators = [.. setValidators];
    }

    /// <summary>
    /// Creates a validated instance of a DomainSet or its derivative.
    /// Validates each item using the provided set validators before adding to the collection.
    /// If any item fails validation, an error result is returned.
    /// </summary>
    /// <typeparam name="TImpl"></typeparam>
    /// <param name="items"></param>
    /// <param name="setValidators"></param>
    /// <returns></returns>
    protected static Result<TImpl> CreateValidated<TImpl>(IEnumerable<T> items, params ISetValidator<T>[] setValidators)
        where TImpl : DomainSetBase<T>, new()
    {
        // Create a new instance of the derived class
        var instance = new TImpl();
        // Add the set validators to the instance
        instance._setValidators.AddRange(setValidators);

        // Add the items to the instance, returning any error encountered
        var result = instance.AddRange(items);

        // If adding the items failed, return the error
        if (!result.IsSuccess) return Result<TImpl>.Error(result.GetErrorMessage());

        // Return the instance if all items were added successfully
        return Result<TImpl>.Success(instance);
    }

    // IReadOnlyCollection implementation
    public int Count => _items.Count;
    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Adds an item to the collection
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public Result Add(T item)
    {
        // Validate the item using all set validators
        foreach (var validator in _setValidators)
        {
            // Validate the item against the current items in the set
            var validationResult = validator.Validate(item, _items);

            // If validation failed, return the error
            if (!validationResult.IsSuccess) return validationResult;
        }

        // Add the item to the collection if it does not already exist
        bool added = _items.Add(item);

        // If the item was not added, it means it already exists in the collection
        if (!added) return Result.Error("Item already exists in the collection.");

        // Return success
        return Result.Success();
    }

    /// <summary>
    /// Removes an item from the collection
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public Result Remove(T item)
    {
        // Remove the item from the collection if it exists
        bool removed = _items.Remove(item);

        // If the item was not removed, it means it does not exist in the collection
        if (!removed) return Result.Error("Item does not exist in the collection.");

        // Return success
        return Result.Success();
    }

    /// <summary>
    /// Replaces all items in the collection with the given items
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    public Result ReplaceAll(IEnumerable<T> items)
    {
        // Create a new set for languages
        _items.Clear();

        // Add new languages
        foreach (var item in items)
        {
            _items.Add(item);
        }

        // Return success
        return Result.Success();
    }

    /// <summary>
    /// Checks if the collection contains the given item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item) => _items.Contains(item);

    /// <summary>
    /// Returns the items as an enumerable
    /// </summary>
    /// <returns></returns>
    public IEnumerable<T> AsEnumerable() => _items.AsEnumerable();

    /// <summary>
    /// Returns the items as a read-only set
    /// </summary>
    /// <returns></returns>
    public IReadOnlySet<T> AsReadOnlySet() => _items.ToImmutableHashSet();

    /// <summary>
    /// Adds a range of items to the collection
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    public Result AddRange(IEnumerable<T> items)
    {
        // Add each item and return the first error if any
        foreach (var item in items)
        {
            // Try to add the item  
            var result = Add(item);

            // If adding the item failed, return the error
            if (!result.IsSuccess) return result;
        }

        // Return success
        return Result.Success();
    }

    /// <summary>
    /// Finds an item in the collection that matches the given predicate
    /// </summary>
    /// <param name="match"></param>
    /// <returns></returns>
    public Result<T> Find(Predicate<T> match)
    {
        // Find the first matching item
        var item = _items.FirstOrDefault(i => match(i));

        // If no item was found, return an error
        if (item == null) return Result<T>.Error("Item not found.");

        // Return the item
        return Result<T>.Success(item);
    }

    /// <summary>
    /// Finds all items in the collection that match the given predicate
    /// </summary>
    /// <param name="match"></param>
    /// <returns></returns>
    public Result<T[]> FindAll(Predicate<T> match)
    {
        // Find all matching items
        var items = _items.Where(i => match(i)).ToArray();

        // Return the items
        return Result<T[]>.Success(items);
    }
}
