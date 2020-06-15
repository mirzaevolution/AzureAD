using System;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.Azure.Services.AppAuthentication;
namespace EventHubReceiver1
{
    class Program
    {
        private static string _msiStorageAddress = "https://storage.azure.com/";
        private static string _eventHubNamespace = "sb://skylineeventhub.servicebus.windows.net";
        private static string _eventHubPath = "coreeventhub";
        private static string _storageContainerForLeasing = "eventhub";

        static async Task Init()
        {
            Console.WriteLine("Authenticating...");
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            string token = await azureServiceTokenProvider.GetAccessTokenAsync(_msiStorageAddress);
            TokenCredential tokenCredential = new TokenCredential(token);
            CloudStorageAccount cloudStorageAccount = new CloudStorageAccount(new StorageCredentials(tokenCredential), "storageevolution", "core.windows.net", true);

            TokenProvider tokenProvider = TokenProvider.CreateManagedIdentityTokenProvider();
            EventProcessorHost eventProcessorHost = new EventProcessorHost(
                    hostName: Guid.NewGuid().ToString(),
                    endpointAddress: new Uri(_eventHubNamespace),
                    eventHubPath: _eventHubPath,
                    consumerGroupName: PartitionReceiver.DefaultConsumerGroupName,
                    tokenProvider: tokenProvider,
                    cloudStorageAccount: cloudStorageAccount,
                    leaseContainerName: _storageContainerForLeasing

            );
            Console.WriteLine("Authenticated.");
            await eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>();
            Console.WriteLine("Press ENTER to quit");
            Console.ReadLine();
            await eventProcessorHost.UnregisterEventProcessorAsync();
        }
        static void Main(string[] args)
        {
            Init().Wait();
        }
    }
}
