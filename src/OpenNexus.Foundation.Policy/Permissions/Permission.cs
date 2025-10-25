namespace OpenNexus.Foundation.Policy.Permissions;

/// <summary>
/// Represents a permission within the system.
/// </summary>
public readonly struct Permission : IEquatable<Permission>
{
    /// <summary>
    /// The unique key of the permission.
    /// </summary>
    public readonly string Key { get; }

    /// <summary>
    /// The description of the permission.
    /// </summary>
    public readonly string? Description { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Permission"/> struct.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="description"></param>
    /// <exception cref="ArgumentException"></exception>
    public Permission(string key, string? description)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Permission key cannot be null or whitespace.", nameof(key));
        }

        Key = key.Trim().ToLowerInvariant();
        Description = description;
    }

    // Override equality members
    public override readonly string ToString() => Key;
    public override readonly int GetHashCode() => Key.GetHashCode(StringComparison.OrdinalIgnoreCase);
    public override readonly bool Equals(object? obj) => obj is Permission other && Equals(other);

    // Implement IEquatable<Permission>
    public readonly bool Equals(Permission other) => string.Equals(Key, other.Key, StringComparison.OrdinalIgnoreCase);

    // Define equality operators
    public static bool operator ==(Permission left, Permission right) => left.Equals(right);
    public static bool operator !=(Permission left, Permission right) => !(left == right);
}