using Microsoft.AspNetCore.Mvc;
using Weatherbroker.Models;
using Weatherbroker.Models.weather_data;
using Microsoft.EntityFrameworkCore;

namespace Weatherbroker.controller;
[ApiController]
// [Route("[controller]")]
[Route("api")]
public class ApiController : ControllerBase
{
    private readonly AppDbContext _context;

    public ApiController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpPost("data")]
    public async Task<IActionResult> PostData([FromBody] weather_raw data)
    {
        if (data == null)
        {
            return BadRequest("Die empfangenen Daten sind ungültig.");
        }

        if (data.luftfeuchte == 0 || data.temperature == 0)
        {
            return Ok(new { message = "Daten fehlerhaft", data });
        }else
        {
            // Speichere neue Rohdaten
            _context.weather_raw.Add(data);
            await _context.SaveChangesAsync();
            Console.WriteLine("Wo ist dieser SHIT=?? " +data.luftfeuchte);
            // Aktualisiere oder erstelle weather_day
            var today = DateTime.Today;
            var dayEntry = await _context.Set<weather_day>()
                .FirstOrDefaultAsync(wd => wd.day == today.Day && wd.month == today.Month && wd.year == today.Year);
            if (dayEntry == null)
            {
                dayEntry = new weather_day();
                _context.Add(dayEntry);
            }
            // Aktualisiere Min/Max
            dayEntry.maxTemp = Math.Max(dayEntry.maxTemp, data.temperature);
            dayEntry.minTemp = Math.Min(dayEntry.minTemp, data.temperature);
            dayEntry.maxHum = Math.Max(dayEntry.maxHum, data.luftfeuchte);
            dayEntry.minHum = Math.Min(dayEntry.minHum, data.luftfeuchte);
            // Berechne Durchschnittswerte neu
            var allToday = await _context.weather_raw
                .Where(w => w.time.Date == today)
                .ToListAsync();
            dayEntry.avgTemp = allToday.Average(w => w.temperature);
            dayEntry.avgHum = (float) allToday.Average(w => w.luftfeuchte);
            await _context.SaveChangesAsync();
            // var stats = await _context.Set<weather_stats>().FirstOrDefaultAsync();
            var stats = await _context.weather_stats.FindAsync(1);
            if (stats ==null)
            {
                stats = new weather_stats{Id = 1};
                _context.Add(stats);
            }

            var all = await _context.weather_raw.ToListAsync();
            stats.anz = all.Count;
            // stats.maxTemp = Math.Max(stats.maxTemp, data.temperature);
            // stats.minTemp = Math.Min(stats.minTemp, data.temperature);
            // stats.maxHum = Math.Max(stats.maxHum, data.luftfeuchte);
            // stats.minHum = Math.Min(stats.minHum, data.luftfeuchte);

            stats.maxTemp = all.Max(raw => raw.temperature);
            stats.minTemp = all.Min(raw =>raw.temperature );
            stats.maxHum  = all.Max(raw =>raw.luftfeuchte ) ;
            stats.minHum  = all.Min(raw => raw.luftfeuchte);
            stats.avgTemp = all.Average(raw => raw.temperature);
            stats.avgHum =  (float) all.Average(raw => raw.luftfeuchte);
            await _context.SaveChangesAsync();
            // Wenn du das letzte Element zurückgeben möchtest, kannst du das so machen:
            return Ok(new { message = "Daten gespeichert", data });
        }
    }
    
    [HttpGet("getdata")]
    public async Task<IActionResult> GetData()
    {
        var data = await _context.weather_raw
            .OrderBy(raw =>raw.Id )
            .LastOrDefaultAsync();
        return Ok(new
        {
            temp = data.temperature,
            luft = data.luftfeuchte
        });
    }
    
    
    [HttpGet("stats")]
    public async Task<IActionResult> Getstats()
    {
        var data = await _context.weather_stats.FindAsync(1);
        Console.WriteLine("Eigentlich sollen hier stats sein: "+ data.anz);
        return Ok(new
        {
            anz = data.anz,
            tavg = data.avgTemp,
            tmax = data.maxTemp,
            tmin = data.minTemp,
            havg = data.avgHum,
            hmax = data.maxHum,
            hmin = data.minHum
        });
    }
    
    [HttpGet("monthly-averages")]
    public async Task<IActionResult> GetMonthlyAverages()
    {
        var data = await _context.weather_day
            .GroupBy(w => new { w.month, w.year })
            .Select(g => new {
                Month = g.Key.month,
                Year = g.Key.year,
                AvgTemp = g.Average(w => w.avgTemp),
                AvgHum = g.Average(w => w.avgHum)
            })
            .OrderBy(g => g.Year).ThenBy(g => g.Month)
            .ToListAsync();

        return Ok(data);
    }

    [HttpGet("day-minmax")]
    public async Task<IActionResult> GetTodayMinMax()
    {
        var today = DateTime.Today;

        var entry = await _context.weather_day
            .FirstOrDefaultAsync(w =>
                w.day == today.Day &&
                w.month == today.Month &&
                w.year == today.Year);

        if (entry == null)
        {
            return NotFound(new { message = "Keine Daten für heute gefunden." });
        }

        return Ok(new
        {
            Date = $"{entry.day}.{entry.month}.{entry.year}",
            MinTemp = entry.minTemp,
            MaxTemp = entry.maxTemp,
            MinHum = entry.minHum,
            MaxHum = entry.maxHum
        });
    }


}