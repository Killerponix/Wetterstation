namespace Weatherbroker.Models.weather_data;

public class weather_raw
{
    public int Id { get; set; }
    public DateTime time { get; set; } = DateTime.Now;
    public float temperature { get; set; }
    public int luftfeuchte { get; set; }
    
}