namespace Domain;

public class TemperatureReading
{
    public int Id { get; set; }
    public float Value { get; set; }
    public DateTime ReadingTime { get; set; }
}

public class LightReading
{
    public int Id { get; set; }
    public int Value { get; set; }
    public DateTime ReadingTime { get; set; }
}