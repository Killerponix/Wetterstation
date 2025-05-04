using Weatherbroker.Models;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Weatherbroker;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Logging.AddConsole();
builder.Services.AddControllers();



var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseMySql("server=192.168.178.28;database=weather;user=remote;password=123",new MySqlServerVersion(new Version(10,11))));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(10, 11))));


// Configure the HTTP request pipeline.
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
// app.UseHttpsRedirection();



app.Run();
