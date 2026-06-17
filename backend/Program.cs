using Microsoft.EntityFrameworkCore;
using CryptoTrackerAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Registro Controladores
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registro la conexión a SQL Server usando Entity Framework
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registro HttpClient para que los controladores puedan consumir CriptoYa
builder.Services.AddHttpClient();

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

var app = builder.Build();

// Configuro el pipeline de peticiones HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Habilito CORS en la aplicación
app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();