
using Data.Repository.Implementation;
using Data.Repository.Interfaces;
using Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Service.Models;
using Service.Services;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Microsoft.AspNetCore.Rewrite;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Service.Services.Interface;
using Service.Services.Implementation;
using Data.Repository.Implementation;
using Stripe;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddControllers();
using var conn = new SqlConnection(builder.Configuration.GetConnectionString("Server=tcp:acco-mart.database.windows.net,1433;Initial Catalog=Accomart;Persist Security Info=False;User ID=anmol;Password=kamal.kumar@799;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;\";"));


//Services and Repo 
builder.Services.AddScoped<IProductService, Service.Services.Implementation.ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IUserManagement, UserManagement>();

builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer("Server=tcp:acco-mart.database.windows.net,1433;Initial Catalog=AccoMart;Persist Security Info=False;User ID=anmol;Password=kamal.kumar@799;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
});




builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.Configure<IdentityOptions>(options => options.SignIn.RequireConfirmedEmail = true);
builder.Services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromHours(10));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };
});

//Email COnfiguration
var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
StripeConfiguration.ApiKey = configuration.GetSection("Stripe:SecretKey").Get<string>();
builder.Services.AddSingleton(emailConfig);
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserManagement,UserManagement>();

//Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(c => {
    var config = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"), true);
    config.AbortOnConnectFail = false; // Enable retry policy
    return ConnectionMultiplexer.Connect(config);
});

builder.Services.AddSession();
builder.Services.AddMvc();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
// }
app.UseCors(options =>
{
    options.AllowAnyOrigin()
           .AllowAnyHeader()
           .AllowAnyMethod();
});
app.UseRewriter();  
app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
//app.UseMvc();
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: default,
    pattern: "{controller=Authentication}/{action = Register}");

app.Run();
