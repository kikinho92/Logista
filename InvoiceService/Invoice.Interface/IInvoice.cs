namespace Invoice.Interface;

/// <summary>
/// This service centralizes the management of the invoices 
/// </summary>
public interface IInvoice
{
    const string SERVICE_ROUTE = "invoice";
    const string PING = "ping";


    /// <summary>
    /// Information about an invoice 
    /// </summary>
    /// <param name="id">Internal identifier of the invoince in the system</param>
    /// <param name="reference">External identifier of the invoice in the system</param>
    /// <param name="type">Invoice Type (Simple = 1, Complete = 2)</param>
    /// <param name="issuerName">Name of the invoice issuer</param>
    /// <param name="issuerNif">Nif of the invoice issuer</param>
    /// <param name="status">Current status of the invoice (Pending = null, Registered = true, Registered with errors = false)</param>
    public record Invoice(string id, string reference, int type, string issuerName, string issuerNif, bool? status);

    /// <summary>
    /// A invoice is stored in the system. Initially its status will be pending
    /// </summary>
    /// <param name="invoice">Information about the invoice</param>
    /// <returns></returns>
    Task StoreInvoice(Invoice invoice);

    /// <summary>
    /// Provides the details of the invoices in the system. Params will determinate the amount of invoices provided and its order
    /// </summary>
    /// <param name="pageSize"></param>
    /// <param name="page"></param>
    /// <param name="order"></param>
    /// <param name="pending"></param>
    /// <returns></returns>
    Task<List<Invoice>> GetInvoices(int pageSize, int page, string order, bool? pending);
    const string PAGE_SIZE_QUERY = "pagesize";
    const string PAGE_QUERY = "page";
    const string ORDER_QUERY = "order";
    const string PENDING_QUERY = "pending";
}
