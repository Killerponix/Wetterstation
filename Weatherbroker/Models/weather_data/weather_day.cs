using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Weatherbroker.Models.weather_data;

public class weather_day
{
    public int id { get; set; }
    public int day { get; set; } = DateTime.Now.Day;
    public int month { get; set; } = DateTime.Now.Month;
    public int year { get; set; } = DateTime.Now.Year;
    public float maxTemp { get; set; } =float.MinValue;
    public float minTemp { get; set; } =float.MaxValue;
    public int maxHum { get; set; } = 0;
    public int minHum { get; set; } = 100;
    public float avgTemp { get; set; }
    public float avgHum { get; set; }
    
}