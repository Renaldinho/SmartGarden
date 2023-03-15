using System.Text;
using Domain;
using Infrastructure.Database;
using MQTTnet.Client;

namespace Application;

public class MessageHandler 
{
    //Reading enum's
    private readonly string TemperatureTopic = "SmartGarden/Temperature";
    private readonly string LightTopic = "SmartGarden/Light";
    private readonly string AirTopic = "SmartGarden/Air";
    
    
    public DatabaseContext DatabaseContext { get; set; }

    public MessageHandler(DatabaseContext databaseContext)
    {
        DatabaseContext = databaseContext;
    }

    public void HandleMessage(MqttApplicationMessageReceivedEventArgs sender)
    {
        /*
        Console.WriteLine($"amount of readings: {DatabaseContext.TemperatureReadings.ToArray().Length}");
        Console.WriteLine(sender.ApplicationMessage.Topic);
        Console.WriteLine(DatabaseContext.TemperatureReadings.Add(new TemperatureReading()
        {
            ReadingTime = DateTime.Now,
            Value = Double.Parse(Encoding.UTF8.GetString(sender.ApplicationMessage.Payload).ToString())
        }).Entity.Value);
        DatabaseContext.SaveChangesAsync();
        */
        string topic = sender.ApplicationMessage.Topic;
        if (topic.Equals(TemperatureTopic))
        {
            Console.WriteLine("MAtches with temp");
        }
        else if (topic.Equals(AirTopic))
        {
            Console.WriteLine("MAtches with air");
        }
        else if (topic.Equals(LightTopic))
        {
            Console.WriteLine("MAtches with light");
        }

    }
        
        
}
