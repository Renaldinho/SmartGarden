using System.Globalization;
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
        
        string topic = sender.ApplicationMessage.Topic;
        if (topic.Equals(TemperatureTopic))
        {
            CreateTemperatureReading(sender);
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

    private void CreateTemperatureReading(
        MqttApplicationMessageReceivedEventArgs sender)
    {
        float reading = float.Parse(Encoding.UTF8.GetString(sender.ApplicationMessage.Payload),CultureInfo.InvariantCulture);
        Console.WriteLine(DatabaseContext.TemperatureReadings.Add(new TemperatureReading()
        {
            ReadingTime = DateTime.Now,
            Value = reading
        }).Entity.Value);
        DatabaseContext.SaveChangesAsync();
        
    }
}
