using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Primitives;
using System.Text;

namespace SBSender
{
    class Program
    {
        private static readonly string _endpoint = "servicebusevolution.servicebus.windows.net";
        private static readonly string _topicName = "msi_topic";
        private static TopicClient _topicClient;
        private static void Authenticate()
        {
            try
            {

                Console.WriteLine("....Authenticating");
                TokenProvider tokenProvider = TokenProvider.CreateManagedIdentityTokenProvider();
                _topicClient = new TopicClient(_endpoint, _topicName, tokenProvider);
                Console.WriteLine("....Authenticated");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private static async Task SendMessage(string messageText)
        {
            Message message = new Message(Encoding.UTF8.GetBytes(messageText));
            Console.WriteLine("....Sending");
            await _topicClient.SendAsync(message);
            Console.WriteLine("....Sent");

        }
        private static async Task DoLoop()
        {
            while (true)
            {
                Console.Write("\n> ");
                string message = Console.ReadLine();
                if (message.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;

                }
                await SendMessage(message);
            }
        }
        static void Main(string[] args)
        {
            Authenticate();
            DoLoop().Wait();
            _topicClient.CloseAsync().Wait();
            Console.ReadLine();
        }
    }
}
