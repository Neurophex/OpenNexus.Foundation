namespace OpenNexus.Foundation.DDD.Exceptions;

/// <summary>
/// Represents an exception that occurs within a specific domain.
/// </summary>
public class DomainException : global::System.Exception
{
    /// <summary>
    /// Gets the domain where the exception occurred.
    /// </summary>
    public string Domain { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class with a specified domain and error message.
    /// </summary>
    /// <param name="domain">The domain the error occurred in</param>
    /// <param name="message">The message of the error</param>
    public DomainException(string domain, string message) : base(message)
    {
        Domain = domain;
    }
}