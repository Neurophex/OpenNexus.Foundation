using System.Collections.Frozen;
using System.Collections.Immutable;
using OpenNexus.Foundation.DDD.Core;
using OpenNexus.Foundation.Utils;

namespace OpenNexus.Foundation.DDD.Types;

/// <summary>
/// A collection of value objects
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class ValueObjectSet<T>
    where T : ValueObject
{
    // The items of the collection
    private readonly HashSet<T> _items;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueObjectCollection{T}"/> class.
    /// </summary>
    public ValueObjectSet()
    {
        _items = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueObjectCollection{T}"/> class.
    /// </summary>
    /// <param name="items">Populates the collection with the given items</param>
    public ValueObjectSet(IEnumerable<T> items)
    {
        _items = [.. items];
    }

    /// <summary>
    /// Adds an item to the collection
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public Result Add(T item)
    {
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
    /// Gets all items in the collection
    /// </summary>
    public int Count => _items.Count;

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