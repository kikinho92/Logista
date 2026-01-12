using Log.Interface;
using Log.Service.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Log.Service.Controllers;

[ApiController]
[Route(ILogApi.SERVICE_ROUTE)]
public class LogController : ControllerBase
{
    private readonly LogDbContext _dbContext;

    public LogController(LogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [Route(ILogApi.PING)]
    [AllowAnonymous]
    public string Ping()
    {
        return $"{ILogApi.SERVICE_ROUTE} service OK";
    }

    [HttpPost]
    public async Task<ActionResult> WriteAsync(ILogApi.LogTrace trace)
    {
        DoStoreTrace(trace);

        return Ok();
    }

    private void DoStoreTrace(ILogApi.LogTrace trace)
    {
        if (trace == null) return;

        Data.LogTrace logTrace = new()
        {
            Id = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Level = Trunc(trace.level, 10),
            Source = Trunc(trace.source, 50),
            Message = Trunc(trace.message, 2000),
            Thread = Trunc(trace.thread, 50)
        };
        _dbContext.LogTrace.Add(logTrace);
        _dbContext.SaveChangesAsync();
    }

    private string? Trunc(string text, int limit) => text == null || text.Length <= limit ? text : text.Substring(0, limit);
}
