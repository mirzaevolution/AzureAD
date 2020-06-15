using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Primitives;
using System.Text;
using System.Threading;

namespace SBReceiver
{
    class Program
    {
        private static readonly string _endpoint = "servicebusevolution.servicebus.windows.net";
        private static readonly string _topicName = "msi_topic";
        private static readonly string _subName = "ms_sub";
        private static SubscriptionClient _subscriptionClient;
        private static void Authenticate()
        {
            try
            {

                Console.WriteLine("....Authenticating");
                TokenProvider tokenProvider = TokenProvider.CreateManagedIdentityTokenProvider();
                _subscriptionClient = new SubscriptionClient(_endpoint, _topicName, _subName, tokenProvider);
                Console.WriteLine("....Authenticated");
                _subscriptionClient.RegisterMessageHandler(OnMessageReceived, new MessageHandlerOptions(OnErrorOccured)
                {
                    AutoComplete = false,
                    MaxConcurrentCalls = 2
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static Task OnErrorOccured(ExceptionReceivedEventArgs context)
        {
            Console.WriteLine("Error");
            Console.WriteLine(context.Exception.Message);
            return Task.CompletedTask;
        }

        private static async Task OnMessageReceived(Message payload, CancellationToken cancellationToken)
        {
            string message = Encoding.UTF8.GetString(payload.Body);
            Console.WriteLine($"[*] {message}");
            await _subscriptionClient.CompleteAsync(payload.SystemProperties.LockToken);
        }

        static void Main(string[] args)
        {
            Authenticate();
            Console.ReadLine();
        }
    }
}
