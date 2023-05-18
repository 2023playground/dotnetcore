using System.Text.Json;
using Azure.Messaging.ServiceBus;


public class EmailNotificationHandler
{
    private ServiceBusClient client;
    private ServiceBusSender sender;

    public EmailNotificationHandler()
    {
        var clientOptions = new ServiceBusClientOptions()
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };
        // the client that owns the connection and can be used to create senders and receivers
        client = new ServiceBusClient(System.Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTION_STRING"), clientOptions);
        // the sender used to publish messages to the queue
        sender = client.CreateSender(System.Environment.GetEnvironmentVariable("AZURE_SERVICEBUS_QUEUE_NAME"));
    }

    public async Task SendEmailAsync(Film film, List<User> userList)

    {
        // The Service Bus client types are safe to cache and use as a singleton for the lifetime
        // of the application, which is best practice when messages are being published or read
        // regularly.
        //
        // set the transport type to AmqpWebSockets so that the ServiceBusClient uses the port 443. 
        // If you use the default AmqpTcp, you will need to make sure that the ports 5671 and 5672 are open

        // TODO: Replace the <NAMESPACE-CONNECTION-STRING> and <QUEUE-NAME> placeholders
        var clientOptions = new ServiceBusClientOptions()
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };
        client = new ServiceBusClient(System.Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTION_STRING"), clientOptions);
        sender = client.CreateSender(System.Environment.GetEnvironmentVariable("AZURE_SERVICEBUS_QUEUE_NAME"));

        // create a batch 
        using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();


        // try adding a message to the batch
        foreach (var user in userList)
        {
            var info = new NotificationInfo
            {
                Email = user.Email,
                FilmName = film.FilmName,
                UserName = user.FirstName + " " + user.LastName
            };
            var messageBody = JsonSerializer.Serialize(info);
            if (!messageBatch.TryAddMessage(new ServiceBusMessage(messageBody)))
            {
                // if it is too large for the batch
                throw new Exception($"The message is too large to fit in the batch.");
            }
        }

        try
        {
            // Use the producer client to send the batch of messages to the Service Bus queue
            await sender.SendMessagesAsync(messageBatch);
            // Console.WriteLine($"A batch of {numOfMessages} messages has been published to the queue.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            // Calling DisposeAsync on client types is required to ensure that network
            // resources and other unmanaged objects are properly cleaned up.
            await sender.DisposeAsync();
            await client.DisposeAsync();
        }
    }

}
