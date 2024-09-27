namespace MatrixMultiply.Exceptions;

/// <summary>
/// Exception for incorrect input matrix.
/// </summary>
public class IncorrectInputMatrix : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IncorrectInputMatrix"/> class.
    /// </summary>
    public IncorrectInputMatrix()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IncorrectInputMatrix"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    public IncorrectInputMatrix(string message)
        : base(message)
    {
    }
}
