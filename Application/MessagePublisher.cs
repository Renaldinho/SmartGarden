using Domain;
using Infrastructure.Database;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace Application;

public class MessagePublisher
{
    private IMqttClient Client { get; set; }
    private DatabaseContext  DatabaseContext { get; set; }

    public MessagePublisher(IMqttClient client, DatabaseContext databaseContext)
    {
        Client = client;
        DatabaseContext = databaseContext;
    }

    public async Task PublishMessageUpdateAsync()
    {
        try
        {
            TemperatureReading latestTempReading = DatabaseContext.TemperatureReadings.ToArray().LastOrDefault();
            String messagePayload = latestTempReading.Value.ToString();
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("my/topic")
                .WithPayload(messagePayload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();
            if (Client.IsConnected)
            {
                await Client.PublishAsync(message);
            }
            Console.WriteLine(messagePayload);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}