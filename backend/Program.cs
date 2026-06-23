using Microsoft.EntityFrameworkCore;
using CryptoTrackerAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Registro Controladores
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.NumberHandling =
            System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString |
            System.Text.Json.Serialization.JsonNumberHandling.WriteAsString;

        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    }); builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registro la conexión a SQL Server usando Entity Framework
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuro CORS para permitir peticiones desde el Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddHttpClient();

var app = builder.Build();

// Configuro el pipeline de peticiones HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowVueApp");

app.UseAuthorization();
app.MapControllers();
app.Run();