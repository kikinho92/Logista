using Invoice.Interface;

namespace Register.Interface;

public interface IRegister
{
    const string SERVICE_ROUTE = "register";
    const string PING = "ping";

    /// <summary>
    /// Pending invoices will be register in an external service given by Governmental Tax Agency.
    /// Invoices will be mark as Registered or Registered with errors
    /// </summary>
    /// <returns></returns>
    Task RegisterInvoices();
}
