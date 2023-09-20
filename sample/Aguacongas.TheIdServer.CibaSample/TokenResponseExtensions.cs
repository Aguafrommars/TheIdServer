using IdentityModel;
using IdentityModel.Client;
using System.Text;
using System.Text.Json;

namespace Aguacongas.TheIdServer.CibaSample
{
    public static class TokenResponseExtensions
    {
        public static void Show(this TokenResponse response)
        {
            if (!response.IsError)
            {
                "Token response:".ConsoleGreen();
                Console.WriteLine(response.Json);

                if (response.AccessToken is not null && response.AccessToken.Contains(".") == true)
                {
                    "\nAccess Token (decoded):".ConsoleGreen();

                    var parts = response.AccessToken.Split('.');
                    var header = parts[0];
                    var payload = parts[1];

                    Console.WriteLine(PrettyPrintJson(Encoding.UTF8.GetString(Base64Url.Decode(header))));
                    Console.WriteLine(PrettyPrintJson(Encoding.UTF8.GetString(Base64Url.Decode(payload))));
                }
            }
            else
            {
                if (response.ErrorType == ResponseErrorType.Http)
                {
                    "HTTP error: ".ConsoleGreen();
                    Console.WriteLine(response.Error);
                    "HTTP status code: ".ConsoleGreen();
                    Console.WriteLine(response.HttpStatusCode);
                }
                else
                {
                    "Protocol error response:".ConsoleGreen();
                    Console.WriteLine(response.Raw);
                }
            }
        }

        public static string PrettyPrintJson(this string raw)
        {
            var doc = JsonDocument.Parse(raw).RootElement;
            return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
