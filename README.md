
### Overview
---
In this exercise you learn how to:

- Create a Service Bus namespace, and queue, using the Azure CLI

- Create a .NET console application to send and receive messages from the queue
   
### Key Aspects
---
- Azure Portal

- Azure Could Shell

- Azure CLI

- Azure Service Bus

- Visual Studio Code


### Environment
---
Microsoft Azure Portal
- Valid Subscription

Visual Studio Code
- C# extension
- Dotnet 8.0 or greater

### Actions
---
Connect to Azure
- Sign in to the Azure portal
- Open the Azure Cloud Shell using the Bash

Prepare your environment
- Set environment variables
```
DEFAULT_CURRENTDATETIME="20240522194000"
DEFAULT_LOCATION="eastus2"
DEFAULT_RESOURCEGROUP_NAME="myresourcegroup${DEFAULT_CURRENTDATETIME}"
AZURESERVICEBUS_NAMESPACE_NAME="MYNAMESPACE${DEFAULT_CURRENTDATETIME}"
AZURESERVICEBUS_QUEUE_NAME="MYQUEUE${DEFAULT_CURRENTDATETIME}"
```

- Create a resource group
```
az group create --name ${DEFAULT_RESOURCEGROUP_NAME} --location ${DEFAULT_LOCATION}
```

Create an Azure Service Bus and its Namespace
- Create a Service Bus messaging namespace
```
az servicebus namespace create --resource-group ${DEFAULT_RESOURCEGROUP_NAME} --name ${AZURESERVICEBUS_NAMESPACE_NAME} --location ${DEFAULT_LOCATION}
```

- Create an Azure Service Bus queue
```
az servicebus queue create --resource-group ${DEFAULT_RESOURCEGROUP_NAME} --namespace-name ${AZURESERVICEBUS_NAMESPACE_NAME} --name ${AZURESERVICEBUS_QUEUE_NAME}
```

Retrieve the connection string for the Service Bus Namespace
- Copy the Primary Connection in RootManageSharedAccessKey from the Shared Access Policies 
```
Endpoint=sb://abc
```

Create a console app to send messages to the queue - Part 01
- Create and access a project folder
```
md Lab-Azure-AzureServiceBus-SendingAndReceivingMessagesOnConsoleApp
cd Lab-Azure-AzureServiceBus-SendingAndReceivingMessagesOnConsoleApp
```

- Open Visual Studio Code using the project folder directory as the base
```
code .
```

- Open the terminal in Visual Studio Code

- Create a new console application
```
dotnet new console
```

- Add the library Azure.Messaging.ServiceBus to console app
```
dotnet add package Azure.Messaging.ServiceBus
```

- Define Program.cs 
```
// the client that owns the connection and can be used to create senders and receivers
ServiceBusClient client;

// the sender used to publish messages to the queue
ServiceBusSender sender;

// Create the clients that we'll use for sending and processing messages.
client = new ServiceBusClient(connectionString);
sender = client.CreateSender(queueName);

// create a batch 
using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

for (int i = 1; i <= 3; i++)
{
    // try adding a message to the batch
    if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
    {
        // if an exception occurs
        throw new Exception($"Exception {i} has occurred.");
    }
}

try
{
    // Use the producer client to send the batch of messages to the Service Bus queue
    await sender.SendMessagesAsync(messageBatch);
    Console.WriteLine($"A batch of three messages has been published to the queue.");
}
finally
{
    // Calling DisposeAsync on client types is required to ensure that network
    // resources and other unmanaged objects are properly cleaned up.
    await sender.DisposeAsync();
    await client.DisposeAsync();
}

Console.WriteLine("Follow the directions in the exercise to review the results in the Azure portal.");
Console.WriteLine("Press any key to continue");
Console.ReadKey();
```

- Compile the project
```
dotnet build
```

- Execute the program
```
dotnet run
```

Review results
- Select the Service Bus Explorer in the Service Bus Queue navigation pane
- Select Peek from the start and the three messages that were sent appear

Create a console app to send messages to the queue - Part 02
- Redefine Program.cs
```
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


try
{
     // Use the producer client to send the batch of messages to the Service Bus queue
     await sender.SendMessagesAsync(messageBatch);
      Console.WriteLine($"A batch of three messages has been published to the queue.");
}
finally
{
    // Calling DisposeAsync on client types is required to ensure that network
    // resources and other unmanaged objects are properly cleaned up.
    await sender.DisposeAsync();
    await client.DisposeAsync();    
}

Console.WriteLine("Follow the directions in the exercise to review the results in the Azure portal.");
Console.WriteLine("Press any key to continue");

// Consumer
ServiceBusProcessor processor;

// create a processor that we can use to process the messages
processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

try
{
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
}
finally
{
    // Calling DisposeAsync on client types is required to ensure that network
    // resources and other unmanaged objects are properly cleaned up.
    await processor.DisposeAsync();
    await client.DisposeAsync();
}

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
```

- Compile the project
```
dotnet build
```

- Execute the program
```
dotnet run
```
 
Publish the code
- Create the repository Lab-Azure-AzureServiceBus-SendingAndReceivingMessagesOnConsoleApp
- Generate a new specific/dedicated personal access token (Fine-grained tokens)
- Publish the code to GitHub

Clean up resources
- Delete the Resource Groups and its content
```
az group delete --name ${DEFAULT_RESOURCEGROUP} --no-wait
```

### Media
---
![image](https://github.com/ViCunha/Lab-Azure-AzureServiceBus-SendingAndReceivingMessagesOnConsoleApp/assets/65992033/cd45d283-b447-4fb4-96aa-7ad8096424b5)
---
![image](https://github.com/ViCunha/Lab-Azure-AzureServiceBus-SendingAndReceivingMessagesOnConsoleApp/assets/65992033/5fa0b41b-eadb-45ce-9254-f003af02aaaf)

---
![image](https://github.com/ViCunha/Lab-Azure-AzureServiceBus-SendingAndReceivingMessagesOnConsoleApp/assets/65992033/c2a0d566-46c2-43d5-ab76-0c643db6b326)


### References
---
- [Exercise: Send and receive message from a Service Bus queue by using .NET.](https://learn.microsoft.com/en-us/training/modules/discover-azure-message-queue/6-send-receive-messages-service-bus)
