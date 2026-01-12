using Invoice.Interface;
using Invoice.Service.Data;
using Log.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.Service.Controllers;

[ApiController]
[Route(IInvoice.SERVICE_ROUTE)]
public class InvoiceController : ControllerBase
{

    private readonly InvoiceDbContext _dbContext;
     private readonly ILogApi _log;

    public InvoiceController(InvoiceDbContext dbContext, ILogApi log)
    {
        _dbContext = dbContext;
        _log = log;
    }

    [HttpGet]
    [Route(IInvoice.PING)]
    [AllowAnonymous]
    public string Ping()
    {
        return $"{IInvoice.SERVICE_ROUTE} service OK";
    }

    [HttpPut]
    public async Task<ActionResult> StoreInvoice(IInvoice.Invoice invoice)
    {
        try
        {
            // Validating invoice.
            string[] errors = ValidateInvoice(invoice);
            if (errors.Length > 0) return BadRequest($"Error - Invalid invoice. {string.Join(", ", errors)}");

            Data.Invoice invoiceData;
            if (invoice.id == null)
            {
                // New inovice, saving new entity in the system
                 invoiceData = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Date = DateTime.UtcNow,
                        Reference = invoice.reference,
                        Type = (Data.Type)invoice.type,
                        IssuerName = invoice.issuerName,
                        IssuerNif = invoice.issuerNif,
                        RegisterStatus = invoice.status
                    };

                _dbContext.Invoice.Add(invoiceData);

                _ = _log.Info(IInvoice.SERVICE_ROUTE, $"New invoice with id {invoice.id} has been stored succesfully");

                //_notif.FireNotification(NEW_INVOICE_SOTED);
                // Notifications should be sent by NotifService, an independence microservices, which will handle sending all notifications between services. Abstract implementation
                // In this case, when a notification is fired, it will have an associated endpoint which will be executed (http://localhost:8003/register). 
            }
            else
            {
                // Existing invoice, so this need to be updated (using for update register status currently)
                invoiceData = _dbContext.Invoice.Where(i => i.Id == invoice.id).FirstOrDefault();
                
                if(invoiceData == null) return BadRequest($"Error - Not found invoice");

                invoiceData.Reference = invoice.reference;
                invoiceData.Type = (Data.Type)invoice.type;
                invoiceData.IssuerName = invoice.issuerName;
                invoiceData.IssuerNif = invoice.issuerNif;
                invoiceData.RegisterStatus = invoice.status;

                _ = _log.Info(IInvoice.SERVICE_ROUTE, $"Invoice with id {invoice.id} has been updated succesfully");
            }

            _dbContext.SaveChanges();

            return Ok();
        }
        catch (Exception e)
        {
            _ = _log.Error(IInvoice.SERVICE_ROUTE, $"Internal error in StoreInvoice(): {e.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal error. {e.Message}");
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<IInvoice.Invoice>>> GetInvoices(int pageSize, int page, string order, bool pending)
    {
        try
        {
            // Getting invoices in the system filtered by status. status == null will bring pending invoices and status == true the registed ones
            IEnumerable<Data.Invoice> invoicesData = _dbContext.Invoice.Where(i => i.RegisterStatus == pending);

            //Request decides the order of the invoices given
            switch (order)
            {
                case "Date":
                    invoicesData = invoicesData.OrderBy(i => i.Date);
                    break;
                case "Type":
                    invoicesData = invoicesData.OrderBy(i => i.Type);
                    break;
                case "Status":
                    invoicesData = invoicesData.OrderBy(i => i.RegisterStatus);
                    break; 
                default:
                    break;
            }
            //Pagination requested
            invoicesData = invoicesData
                    .Skip(page * pageSize)
                    .Take(pageSize).ToList();

            List<IInvoice.Invoice> invoices = new();
            foreach (Data.Invoice invoice in invoicesData)
            {
                invoices.Add(new IInvoice.Invoice(invoice.Id,
                                invoice.Reference,
                                (int)invoice.Type,
                                invoice.IssuerName,
                                invoice.IssuerNif,
                                invoice.RegisterStatus
                                ));
            }

            return Ok(invoices);
        }
        catch (Exception e)
        {
            _ = _log.Error(IInvoice.SERVICE_ROUTE, $"Internal error in GetInvoices(): {e.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal error. {e.Message}");
        }
    }

    private static string[] ValidateInvoice(IInvoice.Invoice invoice)
    {
        List<string> errors = new();

        if(invoice == null)
        {
            errors.Add("Invoice can not be null");
            return errors.ToArray();
        }

        if(string.IsNullOrEmpty(invoice.reference)) errors.Add("Invoice reference is a required field");
        if(string.IsNullOrEmpty(invoice.issuerName)) errors.Add("Invoice issuer name is a required field, it can not be empty");
        if(string.IsNullOrEmpty(invoice.issuerNif)) errors.Add("Invoice issuer nif is a required field, it can not be empty");

        return errors.ToArray();
    }
}
