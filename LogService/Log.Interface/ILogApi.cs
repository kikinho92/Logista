using System.Threading.Tasks;

namespace Log.Interface;

/// <summary>
/// Logging service API
/// The log service is in charge of collecting logging message from other services
/// </summary>
public interface ILogApi
{
    const string SERVICE_ROUTE = "log";
    const string PING = "ping";

    /// <summary>
    /// A single logging message
    /// </summary>
    /// <param name="source">Service name trace come from</param>
    /// <param name="level">Importance level of the log message(DEBUG, INFO, WARN, ERROR)</param>
    /// <param name="message">Body of the log message</param>
    /// <param name="thread">Thread that has written the message</param>
    public record LogTrace(string source, string level, string message, string thread);

    /// <summary>
    /// Records a new log message
    /// </summary>
    /// <param name="trace">Log message that is going to be insert</param>
    /// <returns></returns>
    Task WriteAsync(LogTrace trace);

    /// <summary>
    /// Registers a new log trace of level DEBUG
    /// </summary>
    /// <returns></returns>
    Task Debug(string source, string message);

    /// <summary>
    /// Registers a new log trace of level INFO
    /// </summary>
    /// <returns></returns>
    Task Info(string source, string message);

    /// <summary>
    /// Registers a new log trace of level WARN
    /// </summary>
    /// <returns></returns>
    Task Warn(string source, string message);

    /// <summary>
    /// Registers a new log trace of level ERROR
    /// </summary>
    /// <returns></returns>
    Task Error(string source, string message);

}
