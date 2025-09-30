namespace OpenNexus.Foundation.Primitives;

#region Result

/// <summary>
/// Result class for handling operation results.
/// </summary>
public class Result
{
    private readonly bool _success;
    private readonly string? _errorMessage;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="isSuccess"></param>
    /// <param name="error"></param>
    private Result(bool isSuccess, string? error = null)
    {
        _success = isSuccess;
        _errorMessage = error;
    }

    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess => _success;

    /// <summary>
    /// Gets the error message if the operation was not successful.
    /// </summary>
    public string GetErrorMessage() => _errorMessage ?? "An unknown error occurred.";

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> with a successful result.
    /// </summary>
    public static Result Success() => new Result(true);

    /// <summary>
    /// Creates a new instance of the <see cref="Result"/> class with an error message.
    /// </summary>
    public static Result Error(string error) => new Result(false, error);
}

#endregion

#region GenericResult

/// <summary>
/// Generic Result class for handling operation results.
/// </summary>
/// <typeparam name="T">The type of the value that is returned when the result is successful</typeparam>
public class Result<T> where T : notnull
{
    private readonly T? _value;
    private readonly bool _success;
    private readonly string? _errorMessage;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> class.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="isSuccess"></param>
    /// <param name="error"></param>
    private Result(T? value, bool isSuccess = true, string? error = null)
    {
        _value = value;
        _success = isSuccess;
        _errorMessage = error;
    }

    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public T Value
    {
        // 
        get
        {
            // Check if the operation was successful, if not we trow an exception.
            if (!_success)
            {
                throw new InvalidOperationException("Cannot access Value when Result is not successful.");
            }
            return _value!;
        }
    }

    /// <summary>
    /// Gets wether the operation was successful or not.
    /// </summary>
    public bool IsSuccess
    {
        get { return _success; }
    }

    /// <summary>
    /// Gets the error message if the operation was not successful.
    /// </summary>
    /// <returns></returns>
    public string GetErrorMessage()
    {
        // If the operation was successful, we return an empty string.
        if (_success)
        {
            return string.Empty;
        }

        // If the operation was not successful, we return the error message.
        return _errorMessage ?? "An unknown error occurred.";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> with a successful result.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Result<T> Success(T value) => new Result<T>(value);

    /// <summary>
    /// Creates a new instance of the <see cref="Result{T}"/> class with an error message.
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static Result<T> Error(string error) => new Result<T>(default, false, error);


    /// <summary>
    /// Implicit operator to convert a value of type T to a Result<T>.
    /// </summary>
    /// <param name="value">The value to return the result with</param>
    public static implicit operator Result<T>(T value) => Success(value);
}

#endregion
