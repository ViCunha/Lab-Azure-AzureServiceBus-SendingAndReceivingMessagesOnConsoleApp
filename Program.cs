using Azure.Messaging.ServiceBus;

// connection string to your Service Bus namespace
string connectionString = "";

// name of your Service Bus topic
string queueName = "MYQUEUE20240522194000";

// the client that owns the connection and can be used to create senders and receivers
ServiceBusClient client;

// the sender used to publish messages to the queue
ServiceBusSender sender;

System.Console.WriteLine(">> Begin");


// Create the clients that we'll use for sending and processing messages.
client = new ServiceBusClient(connectionString);
sender = client.CreateSender(queueName);

// create a batch 
using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();


for (int i = 0; i < 4; i++)
{
    // try adding a message to the batch
    if (!messageBatch.TryAddMessage(new ServiceBusMessage(string.Format("{0}", i))))
    {
        // if an exception occurs
        throw new Exception($"Exception {i} has occurred.");        
    }
}

// Use the producer client to send the batch of messages to the Service Bus queue
await sender.SendMessagesAsync(messageBatch);
Console.WriteLine($"A batch of three messages has been published to the queue.");

Console.WriteLine("Follow the directions in the exercise to review the results in the Azure portal.");
Console.WriteLine("Press any key to continue");

// Consumer
ServiceBusProcessor processor;

// create a processor that we can use to process the messages
processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

// add handler to process messages
processor.ProcessMessageAsync += MessageHandler;

// add handler to process any errors
processor.ProcessErrorAsync += ErrorHandler;

// start processing 
await processor.StartProcessingAsync();

Console.WriteLine("Wait for a minute and then press any key to end the processing");
Console.ReadKey();

// stop processing 
Console.WriteLine("\nStopping the receiver...");
await processor.StopProcessingAsync();
Console.WriteLine("Stopped receiving messages");

await processor.DisposeAsync();
await client.DisposeAsync();


// handle received messages
async Task MessageHandler(ProcessMessageEventArgs args)
{
    string body = args.Message.Body.ToString();
    Console.WriteLine($"Received: {body}");

    // complete the message. messages is deleted from the queue. 
    await args.CompleteMessageAsync(args.Message);
}

// handle any errors when receiving messages
Task ErrorHandler(ProcessErrorEventArgs args)
{
    Console.WriteLine(args.Exception.ToString());
    return Task.CompletedTask;
}

System.Console.WriteLine(">> End");
Console.ReadKey();