using System.Text;
using System.Text.Json;
using Invoice.Interface;
using Register.Interface;

namespace Register.Sdk;

public class RegisterCli
{

    private HttpClient _client;
    private string _base;

    public RegisterCli(IHttpClientFactory httpClientFactory, string serviceBaseUrl)
    {
        _client = httpClientFactory.CreateClient("RegisterApi");
        _base = serviceBaseUrl;
    }

    public async Task StoreInvoice(IInvoice.Invoice invoice)
    {
        StringContent data = new StringContent(JsonSerializer.Serialize<IInvoice.Invoice>(invoice), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _client.PutAsync($"{_base}/" +
                    $"{IRegister.SERVICE_ROUTE}/", data);

        // Provide success.
        if (response.IsSuccessStatusCode)
        {
            return;
        }
        // Error.
        throw new Exception(await response.Content.ReadAsStringAsync());
    }

}
