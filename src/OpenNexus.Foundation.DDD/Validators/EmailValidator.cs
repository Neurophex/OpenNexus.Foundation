using System.Text.RegularExpressions;
using OpenNexus.Foundation.Utils;

namespace OpenNexus.Foundation.DDD.Validators;

public static class EmailValidator
{
    // Regular expression to validate email format
#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.

    /// <summary>
    /// Validates the email address format.
    /// </summary>
    /// <param name="email">The email address to validate.</param>
    /// <returns>True if the email is valid, otherwise false.</returns>
    public static bool IsValid(string email)
    {
        // Check if the email is null or empty
        Guard.AgainstNull(email, nameof(email));

        // Use a regular expression to validate the email format
        return EmailRegex.IsMatch(email);
    }
}