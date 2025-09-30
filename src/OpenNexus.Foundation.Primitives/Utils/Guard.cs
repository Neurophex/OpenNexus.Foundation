namespace OpenNexus.Foundation.Primitives;

/// <summary>
/// Guard class for validating method arguments.
/// </summary>
public static class Guard
{
    /// <summary>
    /// Checks if the provided value is null and throws an ArgumentNullException if it is.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="parameterName"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void AgainstNull<T>(T? value, string? parameterName = null) where T : class
    {
        if (value is null)
        {
            var argName = parameterName ?? typeof(T).Name;
            throw new ArgumentNullException(argName, $"{argName} cannot be null.");
        }
    }

    /// <summary>
    ///  Checks if the provided value is null or Empty and throws an ArgumentException if it is.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parameterName"></param>
    /// <exception cref="ArgumentException"></exception>
    public static void AgainstNullOrEmpty(string? value, string parameterName)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException($"{parameterName} cannot be null or empty.", parameterName);
        }
    }

    /// <summary>
    /// checks if the provided value is out of range and throws an ArgumentOutOfRangeException if it is.    
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="parameterName"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static void AgainstOutOfRange(int value, int min, int max, string parameterName)
    {
        if (value < min || value > max)
        {
            throw new ArgumentOutOfRangeException(parameterName, $"The value of {parameterName} must be between {min} and {max}.");
        }
    }

    /// <summary>
    /// checks if the provided value is invalid input and throws an ArgumentException if it is.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="parameterName"></param>
    /// <param name="condition"></param>
    /// <exception cref="ArgumentException"></exception>
    public static void AgainstInvalidInput<T>(T value, string parameterName, Func<T, bool> condition)
    {
        if (!condition(value))
        {
            throw new ArgumentException($"{parameterName} is invalid. Value was {value}.", parameterName);
        }
    }
}
