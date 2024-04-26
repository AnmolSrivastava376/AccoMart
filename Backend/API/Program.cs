using API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Service.Models;
using Service.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
// Create a SqlConnection object using the connection string
using var conn = new SqlConnection(builder.Configuration.GetConnectionString("Server=tcp:acco-mart.database.windows.net,1433;Initial Catalog=Accomart;Persist Security Info=False;User ID=anmol;Password=kamal.kumar@799;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;\";"));
//conn.Open(); // Open the connection

// Configure DbContextOptionsBuilder to use the SqlConnection
builder.Services.AddDbContext<AuthenticationDbContext>(options =>
{
    options.UseSqlServer("Server=tcp:acco-mart.database.windows.net,1433;Initial Catalog=Accomart;Persist Security Info=False;User ID=anmol;Password=kamal.kumar@799;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
});


builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AuthenticationDbContext>().AddDefaultTokenProviders();
builder.Services.Configure<IdentityOptions>(options => options.SignIn.RequireConfirmedEmail = true);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

});

//Email COnfiguration
var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);
builder.Services.AddScoped<IEmailService, EmailService>();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseRouting();
app.MapControllerRoute(
    name: default,
    pattern: "{controller=Test}/{action = Index}");

app.Run();