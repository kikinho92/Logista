using System.ComponentModel.DataAnnotations;

namespace Log.Service.Data;

/// <summary>
/// Data entity: Single loggging message
/// </summary>
public class LogTrace
{
    [StringLength(100)]
    public string Id { get; set; }
    public DateTime Timestamp { get; set; }
    [StringLength(10), Required]
    public string Level { get; set; }
    [StringLength(50)]
    public string Source { get; set; }
    [StringLength(2000)]
    public string Message { get; set; }
    public string Thread { get; set; }
}