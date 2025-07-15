using CGMAnalyzer.API.Services;
using CGMAnalyzerCore.Convert;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Ajouter les services nécessaires à l'application
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CGM Management
builder.Services.AddScoped<ICgmConverter, CgmConverter>();
builder.Services.AddScoped<CgmImageService>();

builder.Services.AddScoped<ICGMLayerDetector, CGMLayerDetector>();
builder.WebHost.UseUrls("https://localhost:7176");
// ? pour que WebRootPath fonctionne :
builder.WebHost.UseWebRoot("wwwroot");
var app = builder.Build();

// Activer Swagger uniquement en développement
if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

// Middleware HTTP
app.UseHttpsRedirection();
app.UseAuthorization();

// Mapper les routes des contrôleurs
app.MapControllers();

// Lancer l'application
app.Run();

