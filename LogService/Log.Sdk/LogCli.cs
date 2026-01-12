using System.Diagnostics.Contracts;
using System.Text;
using System.Text.Json;
using Log.Interface;

namespace Log.Sdk;

/// <summary>
/// Client library for Log service access
/// </summary>
public class LogCli : ILogApi
{
    private HttpClient _client;
    private string _base;

    public LogCli(IHttpClientFactory httpClientFactory, string serviceBaseUrl)
    {
        _client = httpClientFactory.CreateClient("LogApi");
        _base = serviceBaseUrl;
    }
    public async Task WriteAsync(ILogApi.LogTrace trace)
    {
        try
        {
            StringContent data = new StringContent(JsonSerializer.Serialize<ILogApi.LogTrace>(trace), Encoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = await _client.PostAsync($"{_base}/{ILogApi.SERVICE_ROUTE}", data);
        }
        catch (System.Exception)
        {
        }
    }

    public async Task Debug(string source, string message)
    {
        ILogApi.LogTrace trace = new ILogApi.LogTrace(source, "DEBUG", message, Thread.CurrentThread.ManagedThreadId.ToString());
        await WriteAsync(trace);
    }
    public async Task Info(string source, string message)
    {
        ILogApi.LogTrace trace = new ILogApi.LogTrace(source, "INFO", message, Thread.CurrentThread.ManagedThreadId.ToString());
        await WriteAsync(trace);
    }

    public async Task Warn(string source, string message)
    {
        ILogApi.LogTrace trace = new ILogApi.LogTrace(source, "WARN", message, Thread.CurrentThread.ManagedThreadId.ToString());
        await WriteAsync(trace);
    }

    public async Task Error(string source, string message)
    {
        ILogApi.LogTrace trace = new ILogApi.LogTrace(source, "ERROR", message, Thread.CurrentThread.ManagedThreadId.ToString());
        await WriteAsync(trace);
    }
}
