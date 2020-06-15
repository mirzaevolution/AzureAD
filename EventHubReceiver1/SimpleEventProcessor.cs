using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
namespace EventHubReceiver1
{
    public class SimpleEventProcessor : IEventProcessor
    {
        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            if (reason == CloseReason.Shutdown)
            {
                Console.WriteLine("...Shutdown");
            }
            return Task.CompletedTask;
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine($"...[#RECEIVER-1] Partition: {context.PartitionId}");
            return Task.CompletedTask;
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            Console.WriteLine($"...[#RECEIVER-1] Partition: {context.PartitionId} got an error");
            Console.WriteLine(error.ToString());
            return Task.CompletedTask;
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (EventData data in messages)
            {
                string message = Encoding.UTF8.GetString(data.Body);
                Console.WriteLine($"...[#RECEIVER-1] Partition: {context.PartitionId} got a message: {message}");
            }
            await context.CheckpointAsync();
        }
    }
}
