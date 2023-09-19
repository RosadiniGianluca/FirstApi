using FirstApi.Authentication;
using FirstApi.Clients;
using FirstApi.Entities;
using FirstApi.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MySql.EntityFrameworkCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(/*x => x.Filters.Add<ApiKeyAuthenticationFilter>()*/);  // Aggiunge il filtro a ogni controller del progetto.
                                                                                        // In alternativa si può aggiungere il filtro solo ad alcuni controller,
                                                                                        // ad esempio [ServiceFilter(typeof(ApiKeyAuthenticationFilter))] prima della definizione della classe del controller
                                                                                        // oppure ad una singola richiesta HTTP, ad esempio [HttpGet, ServiceFilter(typeof(ApiKeyAuthenticationFilter))]
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

// Aggiungo il servizio per il client del webhook
builder.Services.AddScoped(x => { return new WebhookClient("https://webhook.site/51f50445-72ec-4c01-89f2-847def9b122d"); });

// Aggiungo il servizio per la connessione al database (MySQL)
builder.Services.AddEntityFrameworkMySQL().AddDbContext<MyDbContext>(options => {
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

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
