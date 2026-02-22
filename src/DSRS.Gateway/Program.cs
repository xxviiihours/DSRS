using DSRS.Gateway.Configurations;
using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

using var loggerFactory = LoggerFactory.Create(config => config.AddConsole());
var startupLogger = loggerFactory.CreateLogger<Program>();

startupLogger.LogInformation("Starting web host");


builder.Services.AddServiceConfigurations(startupLogger, builder);

builder.Services.AddFastEndpoints()
                .SwaggerDocument(o =>
                {
                    o.ShortSchemaNames = false;
                    o.DocumentSettings = (document) =>
                    {
                        document.Title = "Daily Stock Redistribution System API";
                        document.Description = "API for the DSRS application";
                        document.Version = "v1";
                    };
                });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedClients",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:5173") // React app URL
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

var app = builder.Build();

app.UseCors("AllowedClients");

await app.UseAppMiddlewareAndSeedDatabase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
