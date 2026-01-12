using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Invoice.Interface;

namespace Invoice.Sdk;

/// <summary>
/// Client library for Log service access
/// </summary>
public class InvoiceCli : IInvoice
{
    private HttpClient _client;
    private string _base;

    public InvoiceCli(IHttpClientFactory httpClientFactory, string serviceBaseUrl)
    {
        _client = httpClientFactory.CreateClient("InvoiceApi");
        _base = serviceBaseUrl;
    }

    public async Task StoreInvoice(IInvoice.Invoice invoice)
    {
        StringContent data = new StringContent(JsonSerializer.Serialize<IInvoice.Invoice>(invoice), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _client.PutAsync($"{_base}/" +
                $"{IInvoice.SERVICE_ROUTE}/", data);

        // Provide success.
        if (response.IsSuccessStatusCode)
        {
            return;
        }
        // Error.
        throw new Exception(await response.Content.ReadAsStringAsync());
    }

    public async Task<List<IInvoice.Invoice>> GetInvoices(int pageSize, int page, string order, bool? pending)
    {
        HttpResponseMessage response = await _client.GetAsync($"{_base}/" +
                    $"{IInvoice.SERVICE_ROUTE}" +
                    $"?{IInvoice.PAGE_SIZE_QUERY}={pageSize}" +
                    $"&{IInvoice.PAGE_QUERY}={page}" +
                    $"&{IInvoice.ORDER_QUERY}={order}" +
                    $"&{IInvoice.PENDING_QUERY}={pending}");

            // Provide success.
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent) return null;
                return await response.Content.ReadFromJsonAsync<List<IInvoice.Invoice>>();
            }
            // Error.
            throw new Exception(await response.Content.ReadAsStringAsync());
    }

   
}

