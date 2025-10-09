// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
namespace Aguacongas.TheIdServer.UI;

/// <summary>
/// Exception thrown when a user clicks on a malicious or invalid return URL link.
/// </summary>
/// <seealso cref="Exception" />
/// <remarks>
/// Initializes a new instance of the <see cref="InvalidReturnUrlException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
/// </remarks>
/// <param name="message">The error message that explains the reason for the exception.</param>
/// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
[Serializable]
public class InvalidReturnUrlException(string message, Exception? innerException) : Exception(message, innerException)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidReturnUrlException"/> class with a default error message.
    /// </summary>
    public InvalidReturnUrlException() : this("invalid return URL")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidReturnUrlException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public InvalidReturnUrlException(string message) : this(message, null)
    {
    }
}
