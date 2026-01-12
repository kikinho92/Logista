using Register.Interface;
using Microsoft.AspNetCore.Mvc;
using Invoice.Interface;
using System.Text.Json;
using System.Text;
using System.Net;
using Log.Interface;
using Microsoft.AspNetCore.Authorization;

namespace Register.Service.Controllers;

[ApiController]
[Route(IRegister.SERVICE_ROUTE)]
public class InvoiceController : ControllerBase
{

    private readonly IInvoice _invoice;
    private readonly ILogApi _log;
    private readonly HttpClient _externalClient;

    private const string EXTERNAL_ENDPOINT = "http://governmentaltaxagency.com/register";

    public InvoiceController(IInvoice invoice, ILogApi log)
    {
        _invoice = invoice;
        _log = log;
        _externalClient = new HttpClient();
    }

    [HttpGet]
    [Route(IRegister.PING)]
    [AllowAnonymous]
    public string Ping()
    {
        return $"{IRegister.SERVICE_ROUTE} service OK";
    }

    [HttpPost]
    public async Task<ActionResult> RegisterInvoices()
    {
        try
        {
            //Getting pending invoices ordered by Date from invoice service
            List<IInvoice.Invoice> invoices = await _invoice.GetInvoices(1000, 0, "Date", null);

            foreach (IInvoice.Invoice invoice in invoices)
            {
                _ = _log.Debug(IRegister.SERVICE_ROUTE, $"Invoice with id {invoice.id} has been sent to external service {EXTERNAL_ENDPOINT} to be registered");
                //Calling exteral service
                IInvoice.Invoice invoiceRegistered = await ExternalRegisterInvoices(invoice);
                string status = invoiceRegistered.status == null ? "Pending" : invoiceRegistered.status.Value ? "Registered" : "Registered with erros";

                _ = _log.Debug(IRegister.SERVICE_ROUTE, $"Invoice with id {invoice.id} has been registered. Adquired status {status}");

                //Invoice service will update the invoice status in the system
                await _invoice.StoreInvoice(invoiceRegistered);
            }

            return Ok();
        }
        catch (Exception e)
        {
            _ = _log.Error(IRegister.SERVICE_ROUTE, $"Internal error in RegisterInvoices(): {e.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal error. {e.Message}");
        }
    }

    private async Task<IInvoice.Invoice> ExternalRegisterInvoices(IInvoice.Invoice invoice)
    {
        StringContent data = new StringContent(JsonSerializer.Serialize<IInvoice.Invoice>(invoice), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _externalClient.PutAsync($"{EXTERNAL_ENDPOINT}/", data);

            // Provide success.
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent) return null;

                string status = await response.Content.ReadAsStringAsync();
                IInvoice.Invoice invoiceRegistered = new IInvoice.Invoice(invoice.id, 
                                                            invoice.reference, 
                                                            invoice.type, 
                                                            invoice.issuerName, 
                                                            invoice.issuerNif,
                                                            status == "Registered");

                return invoiceRegistered;
            }
            // Error.
            throw new Exception(await response.Content.ReadAsStringAsync());
    }

}
