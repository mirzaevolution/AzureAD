using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Security.Claims;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Rewind.ConsoleClientCredentialFlow1
{
    class Program
    {
        private static string _authority = "https://login.microsoftonline.com/eb9925dd-728d-4b14-8d95-04af41b37dd4";
        private static string _clientId = "a256d239-94d3-4e20-a667-e106e2ee9f98";
        private static string _clientSecret = "SAfT7@cGA@cB[lA1PWUGA?yK0DB9ePKM";
        private static string _resource = "api://core";
        private static string _targetApiBaseAddress = "https://localhost:44388";

        private async static Task Init()
        {
            try
            {
                Console.WriteLine("[*]......Authenticating");
                AuthenticationContext authenticationContext = new AuthenticationContext(_authority);
                var result = await authenticationContext.AcquireTokenAsync(_resource, new ClientCredential(_clientId, _clientSecret));
                Console.WriteLine("[!]......Token retrieved, calling the api with provided token");
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_targetApiBaseAddress);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                    var response = await client.GetAsync("/api/LayerOne/GetClientCredentialMessage");
                    if (response.IsSuccessStatusCode)
                    {
                        string responseText = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"[200]......{responseText}");
                    }
                    else
                    {
                        Console.WriteLine($"[{response.StatusCode}]......{response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        static void Main(string[] args)
        {
            Init().Wait();
            Console.ReadLine();
        }
    }
}
