using System.Reflection;
using System.Text;
using FeatureToggle.API.Identity;
using FeatureToggle.Application.RabbitMQEventBus;
using FeatureToggle.Application.Requests.Commands.UserCommands;
using FeatureToggle.Domain.ConfigurationModels;
using FeatureToggle.Domain.Entity.FeatureManagementSchema;
using FeatureToggle.Infrastructure.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// builder.WebHost.ConfigureKestrel(options =>
// {
//     // Listen on HTTP port
//     options.ListenAnyIP(8080);


//     // Listen on HTTPS port (8081), using a certificate
//     options.ListenAnyIP(8081, listenOptions =>
//     {
//         // Specify the certificate file and its password
//         listenOptions.UseHttps("https-certificate.pfx", "12345");
//     });
// });

builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 8081; // Match the HTTPS port used in Kestrel
});
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddIdentityApiEndpoints<User>()
                .AddEntityFrameworkStores<FeatureManagementContext>();


builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<FeatureManagementContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<Authentication>(builder.Configuration.GetSection("Authentication"));
//builder.Services.AddTransient<UserValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AddUserCommandValidator>();

builder.Services.AddSwaggerGen(x =>
{
    x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by the token."
    });
    x.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.@";
    options.User.RequireUniqueEmail = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;
});

builder.Services.AddAuthentication(x =>
                                    {
                                        x.DefaultAuthenticateScheme =
                                        x.DefaultChallengeScheme =
                                        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                                    })
                                    .AddJwtBearer(x =>
                                    {
                                        
                                        x.TokenValidationParameters = new TokenValidationParameters
                                        {
                                            ValidateIssuerSigningKey = true,
                                            IssuerSigningKey = new SymmetricSecurityKey(
                                                Encoding.UTF8.GetBytes(builder.Configuration["Authentication:JWTSecret"]!)),
                                            ValidateIssuer = true, 
                                            ValidIssuer = builder.Configuration["Authentication:Issuer"], 
                                            ValidateAudience = true, 
                                            ValidAudience = builder.Configuration["Authentication:Audience"],
                                            
                                        };
                                    });
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(IdentityData.AdminUserPolicyName, p =>
    p.RequireClaim(IdentityData.AdminUserClaimName, "True"));

builder.Services.AddDbContext<FeatureManagementContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("FeatureManagementDbContext")));
builder.Services.AddDbContext<BusinessContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("FeatureManagementDbContext")));

builder.Services.AddMediatR(x =>
    x.RegisterServicesFromAssembly(Assembly.Load("FeatureToggle.Application"))
    );

builder.Services.AddSingleton<RabbitMQPublisher>(sp =>
{
    string connectionString = builder.Configuration.GetConnectionString("RabbitMQ")!;
    string exchangeName = "FeatureToggleExchange";

    var publisher = new RabbitMQPublisher(exchangeName);
    publisher.InitializeAsync(connectionString!).GetAwaiter().GetResult(); // Block on initialization
    return publisher;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();
app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
