namespace GameServer;

/// <summary>
/// Logger interface provides logging functionality.
/// </summary>
public interface ILogger
{
  /// <summary>
  /// Logs an debug message.
  /// </summary>
  public void Debug(string message);

  /// <summary>
  /// Logs an information message.
  /// </summary>
  public void Info(string message);

  /// <summary>
  /// Logs an warning message.
  /// </summary>
  public void Warning(string message);

  /// <summary>
  /// Logs an error message.
  /// </summary>
  public void Error(string message);
}