

using System.Text;
using Application;
using Domain;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using MQTTnet;
using MQTTnet.Client;

    var mqttThread = new Thread(async () =>
    {
        var mqttFactory = new MqttFactory();
        IMqttClient Client = mqttFactory.CreateMqttClient();
        var options = new MqttClientOptionsBuilder()
            .WithClientId(Guid.NewGuid().ToString())
            .WithCredentials("UGUQ9A0yCU69KVI1u4WDSLeioenQp5R9zzNyZEWqmQpWAWNUOxmw9qClsP2FIHF7", "")
            .WithTcpServer("mqtt.flespi.io", 1883)
            .WithCleanSession()
            .Build();
        await Client.ConnectAsync(options);
        await SubscribeToTopic(Client);

        
        async Task SubscribeToTopic(IMqttClient client)
        {
            var topicFilter = new MqttTopicFilterBuilder()
                .WithTopic("SmartGarden/#")
                .Build();
            await client.SubscribeAsync(topicFilter);
            
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseSqlite("Data source=db.db");
            var db = new DatabaseContext(optionsBuilder.Options);
            db.Database.EnsureCreated();
            MessageHandler messageHandler = new MessageHandler(db);
            
            client.ApplicationMessageReceivedAsync += (sender) => SubscriptionCallback(sender,messageHandler);
            
            
        
        }
        
        static Task SubscriptionCallback(MqttApplicationMessageReceivedEventArgs sender, MessageHandler messageHandler)
        {
            messageHandler.HandleMessage(sender);
            return Task.CompletedTask;
        }
    });
    
    var dbThread = new Thread(() =>
    {
        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
        optionsBuilder.UseSqlite("Data source=db.db");
        using (var db = new DatabaseContext(optionsBuilder.Options))
        {
            db.Database.EnsureCreated();
            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    });
    
    // Start both threads
    mqttThread.Start();
    dbThread.Start();
    
    Console.Write("started");

    // Wait for both threads to complete
    mqttThread.Join();
    dbThread.Join();
    


