using System;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Identity.Client;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AzureADMSALClientCredentialsFlow
{
    class Program
    {
        private static string _authority = "https://login.microsoftonline.com/eb9925dd-728d-4b14-8d95-04af41b37dd4";
        private static string _clientId = "a256d239-94d3-4e20-a667-e106e2ee9f98";
        private static string _clientSecret = "SAfT7@cGA@cB[lA1PWUGA?yK0DB9ePKM";
        private static string _resource = "api://core/.default";
        private static string _targetApiBaseAddress = "https://localhost:44318";

        private static async Task<string> Init()
        {
            IConfidentialClientApplication clientApplication =
                ConfidentialClientApplicationBuilder.Create(_clientId)
                .WithAuthority(_authority)
                .WithClientSecret(_clientSecret)
                .Build();
            Console.WriteLine("Authenticating....");
            var result = await clientApplication.AcquireTokenForClient(new string[] { _resource })
                .ExecuteAsync();
            Console.WriteLine("Token retrieved.");
            return result.AccessToken;

        }
        private static async Task CallApi()
        {
            string token = await Init();
            Console.WriteLine("Calling the api...");
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_targetApiBaseAddress);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync("/api/CoreApi/GetMessageWithRole");
                if (response.IsSuccessStatusCode)
                {
                    string responseText = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response: {responseText}");
                }
                else
                {
                    Console.WriteLine($"Response: {response.StatusCode}");
                }
            }
        }
        static void Main(string[] args)
        {
            CallApi().Wait();
            Console.ReadLine();
        }
    }
}
