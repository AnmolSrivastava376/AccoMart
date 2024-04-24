using API;
using Microsoft.Data.SqlClient;
using System;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
// For production scenarios, consider keeping Swagger configurations behind the environment check
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }
app.UseHttpsRedirection();
string connectionString = app.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")!;
try
{
    // Table would be created ahead of time in production
    using var conn = new SqlConnection(connectionString);
    conn.Open();
    var command = new SqlCommand(
        "CREATE TABLE Persons (ID int NOT NULL PRIMARY KEY IDENTITY, FirstName varchar(255), LastName varchar(255));",
        conn);
    using SqlDataReader reader = command.ExecuteReader();
}
catch (Exception e)
{
    // Table may already exist
    Console.WriteLine(e.Message);
}
app.MapGet("/Person", () =>
{
    var rows = new List<string>();
    using var conn = new SqlConnection(connectionString);
    conn.Open();
    var command = new SqlCommand("SELECT * FROM Persons", conn);
    using SqlDataReader reader = command.ExecuteReader();
    if (reader.HasRows)
    {
        while (reader.Read())
        {
            rows.Add($"{reader.GetInt32(0)}, {reader.GetString(1)}, {reader.GetString(2)}");
        }
    }
    return rows;
})
.WithName("GetPersons");
app.MapPost("/Person", (Person person) =>
{
    using var conn = new SqlConnection(connectionString);
    conn.Open();
    var command = new SqlCommand(
        "INSERT INTO Persons (firstName, lastName) VALUES (@firstName, @lastName)",
        conn);
    command.Parameters.Clear();
    command.Parameters.AddWithValue("@firstName", person.FirstName);
    command.Parameters.AddWithValue("@lastName", person.LastName);
    using SqlDataReader reader = command.ExecuteReader();
})
.WithName("CreatePerson");
app.Run();