using System.Text;
using System.Web;

namespace Aguacongas.TheIdServer.BlazorApp.BFF.Services;

public class AssertionInjectionHandler(AssertionService assertionService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // On vérifie si c'est une requête de Token/PAR
        if (request.Method == HttpMethod.Post &&
            request.RequestUri?.AbsolutePath.Contains("/connect/") == true &&
            request.Content != null)
        {
            var content = await request.Content.ReadAsStringAsync(cancellationToken);
            var formData = HttpUtility.ParseQueryString(content);

            formData["client_assertion_type"] = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
            formData["client_assertion"] = assertionService.CreateClientToken();

            request.Content = new StringContent(formData.ToString()!, Encoding.UTF8, "application/x-www-form-urlencoded");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}