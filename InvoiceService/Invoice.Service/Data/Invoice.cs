using System.ComponentModel.DataAnnotations;

namespace Invoice.Service.Data;

/// <summary>
/// Data entity: Single Invoice
/// </summary>
public class Invoice
{
    [StringLength(100)]
    public string Id { get; set; }
    public DateTime Date { get; set; }
    public string Reference { get; set; }
    public Type Type { get; set; }
    public string IssuerName { get; set; }
    public string IssuerNif { get; set; }
    public bool? RegisterStatus { get; set; }
}

public enum Type
{
    Simple = 1,
    Complete = 2
}