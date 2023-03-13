

using System.Text;
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
            .WithCredentials("t4g3xaH7YwIeQHhcnqevWzRQtjme7jWqb2jy1QtPaZUPw1RHgV10jIGASQCNZo2Y", "")
            .WithTcpServer("mqtt.flespi.io", 1883)
            .WithCleanSession()
            .Build();
        await Client.ConnectAsync(options);
        await SubscribeToTopic(Client);

        
        async Task SubscribeToTopic(IMqttClient client)
        {
            var topicFilter = new MqttTopicFilterBuilder()
                .WithTopic("my/topic")
                .Build();
            await client.SubscribeAsync(topicFilter);

            client.ApplicationMessageReceivedAsync += (sender) => SubscriptionCallback(sender);
        }
        
        static Task SubscriptionCallback(MqttApplicationMessageReceivedEventArgs sender)
        {
            Console.WriteLine(Encoding.UTF8.GetString(sender.ApplicationMessage.Payload));
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            using (var db = new DatabaseContext(optionsBuilder.Options))
            {
                Console.Write(db.TemperatureReadings.Add(new TemperatureReading()
                {
                    ReadingTime = DateTime.Now,
                    Value = Double.Parse(Encoding.UTF8.GetString(sender.ApplicationMessage.Payload).ToString()),
                    Id = -1
                }).Entity.Value);
            }
            return Task.CompletedTask;
        }
    });
    
    var dbThread = new Thread(() =>
    {
        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
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
    


