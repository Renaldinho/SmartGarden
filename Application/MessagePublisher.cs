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
            await publishTemperatureUpdate();
            await publishLightUpdate();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task publishLightUpdate()
    {
        var readingsOfTheLastHour =
            DatabaseContext.LightReadings.Where(tr => tr.ReadingTime.AddHours(1) > DateTime.Now);
        var messagePayload = readingsOfTheLastHour.Average(tr => tr.Value);
        var message = new MqttApplicationMessageBuilder()
            .WithTopic("SmartGarden/ReadingUpdate/Light")
            .WithPayload(String.Format("{0:0.00} lm",messagePayload))
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();
        if (Client.IsConnected)
        {
            await Client.PublishAsync(message);
        }
    }

    private async Task publishTemperatureUpdate()
    {
        var readingsOfTheLastHour =
            DatabaseContext.TemperatureReadings.Where(tr => tr.ReadingTime.AddHours(1) > DateTime.Now);
        var messagePayload = readingsOfTheLastHour.Average(tr => tr.Value);
        var message = new MqttApplicationMessageBuilder()
            .WithTopic("SmartGarden/ReadingUpdate/Temperature")
            .WithPayload(String.Format("{0:0.00}°C",messagePayload))
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();
        if (Client.IsConnected)
        {
            await Client.PublishAsync(message);
        }
    }
}