using FirstApi.Authentication;
using FirstApi.Clients;
using FirstApi.Controllers;
using FirstApi.Entities;
using FirstApi.Middleware;
using FirstApi.Properties;
using FirstApi.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MySql.EntityFrameworkCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(/*x => x.Filters.Add<ApiKeyAuthenticationFilter>()*/);  // Aggiunge il filtro a ogni controller del progetto.                      
                                                                                        // In alternativa si può aggiungere il filtro solo ad alcuni controller
builder.Services.AddScoped<WebhookController>();
// Carica la configurazione da appsettings.json
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

// Prende la configurazione dal file appsettings.json per il servizio Service Bus
builder.Services.Configure<ServiceBusConfig>(configuration.GetSection("ServiceBusConfig"));
builder.Services.Configure<WebhookConfig>(configuration.GetSection("WebhookConfig"));

// Aggiungo il servizio per il client del webhook
builder.Services.AddScoped(x => {
    var webhookConfig = x.GetRequiredService<IOptions<WebhookConfig>>().Value;
    return new WebhookClient(webhookConfig.WebhookUrl);
});

// Aggiungo il servizio per la gestione delle code di Service Bus
builder.Services.AddSingleton<ServiceBusMessageHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key To Access The API",
        Type = SecuritySchemeType.ApiKey,
        Name = "X-Api-Key",
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });
    var scheme = new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "ApiKey"
        },
        In = ParameterLocation.Header
    };
    var requirement = new OpenApiSecurityRequirement
    {
        { scheme, new List<string>() }
    };
    c.AddSecurityRequirement(requirement);
});

// Aggiungo il servizio per l'autenticazione tramite API Key
builder.Services.AddScoped<ApiKeyAuthenticationFilter>();

// Aggiungo il servizio per la connessione al database (MySQL)
builder.Services.AddEntityFrameworkMySQL().AddDbContext<MyDbContext>(options => {
    options.UseMySQL(configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Ottieni una istanza del tuo gestore dei messaggi
var messageHandler = app.Services.GetRequiredService<ServiceBusMessageHandler>();

// Ottieni una istanza del tuo controller
var serviceProvider = app.Services.CreateScope().ServiceProvider;
var webhookController = serviceProvider.GetRequiredService<WebhookController>();

// Avvia il listener utilizzando il metodo del controller
var result = webhookController.StartListenerInternal();

if (result is OkObjectResult okResult && okResult.Value.ToString() == "Listener internal avviato.")
{
    Console.WriteLine("Listener avviato con successo.");
}
else
{
    Console.WriteLine("Errore nell'avvio del listener.");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();  // Middleware per la gestione delle eccezioni centralizzata (ogni eccezione viene gestita da questo middleware),

//app.UseMiddleware<ApiKeyAuthenticationMiddleware>();  // Middleware per l'autenticazione tramite API Key

app.UseAuthorization();

app.MapControllers();

app.Run();