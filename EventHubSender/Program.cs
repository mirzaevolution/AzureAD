using System;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Core;

namespace EventHubSender
{
    class Program
    {
        private static string _hostName = "sb://skylineeventhub.servicebus.windows.net";
        private static string _entityPath = "coreeventhub";
        private static EventHubClient _eventHubClient;
        private static void Initialize()
        {
            _eventHubClient = EventHubClient.CreateWithManagedIdentity(new Uri(_hostName), _entityPath);

        }
        private static void SendMessage(string message)
        {
            EventData eventData = new EventData(Encoding.UTF8.GetBytes(message));
            Console.WriteLine("Sending....");
            _eventHubClient.SendAsync(eventData).Wait();
            Console.WriteLine("Sent.");
        }
        static void Main(string[] args)
        {
            Initialize();
            string message = string.Empty;
            while (true)
            {
                Console.Write("> ");
                message = Console.ReadLine();
                if (message.Trim().Equals("exit", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }
                SendMessage(message);
            }
            _eventHubClient.Close();
            Console.ReadLine();

        }
    }
}
