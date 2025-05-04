using Microsoft.AspNetCore.Mvc;
using Weatherbroker.Models;
using Weatherbroker.Models.weather_data;
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
    public IActionResult PostData([FromBody] weather_raw data)
    {
        if (data == null)
        {
            return BadRequest("Die empfangenen Daten sind ungültig.");
        }
        _context.Add(data);
        _context.SaveChanges();
        return Ok(new {message = "Daten gespeichert",data});
    }
    
}