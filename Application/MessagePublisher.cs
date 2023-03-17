using System.Text.Json;
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
            var readingsOfTheLastHour =
                DatabaseContext.TemperatureReadings.Where(tr => tr.ReadingTime.AddHours(1) > DateTime.Now);
            var averageTempLastHour = readingsOfTheLastHour.Average(tr => tr.Value);
            String messagePayload = averageTempLastHour.ToString();
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("SmartGarden/ReadingUpdate/Temperature")
                .WithPayload(String.Format("{0:0.00}°C",averageTempLastHour))
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();
            if (Client.IsConnected)
            {
                await Client.PublishAsync(message);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}