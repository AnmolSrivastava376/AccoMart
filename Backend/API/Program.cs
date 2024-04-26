using API.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
// Create a SqlConnection object using the connection string
using var conn = new SqlConnection(builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING"));
conn.Open(); // Open the connection

// Configure DbContextOptionsBuilder to use the SqlConnection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(conn);
});

var app = builder.Build();

// For production scenarios, consider keeping Swagger configurations behind the environment check
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }
app.UseCors(options =>
{
    options.AllowAnyOrigin()
           .AllowAnyHeader()
           .AllowAnyMethod();
});
app.UseHttpsRedirection();

app.UseRouting();
app.MapControllerRoute(
    name: default,
    pattern: "{controller=Test}/{action = Index}");

app.Run();