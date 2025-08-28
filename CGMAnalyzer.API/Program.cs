using CGMAnalyzer.API.Services;
using CGMAnalyzer.API.Services.Interfaces;
using CGMAnalyzerCore.Convert;
using CGMAnalyzerCore.Converter.Interface;
using CGMAnalyzerCore.Exporter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.Runtime.ConstrainedExecution;

var builder = WebApplication.CreateBuilder(args);

// Ajouter les services nécessaires à l'application
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuration CORS pour le client WPF
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// CGM Management
builder.Services.AddScoped<ICgmConverter, CgmConverter>();
builder.Services.AddScoped<ICgmImageService,CgmImageService>();
builder.Services.AddScoped<CgmExportService>();
builder.Services.AddScoped<ICGMLayerDetector, CGMLayerDetector>();

// ? pour que UseUrls fonctionne :
builder.WebHost.UseUrls("https://localhost:7176");
// ? pour que WebRootPath fonctionne :
builder.WebHost.UseWebRoot("wwwroot");

// Configuration des logs
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Configuration des fichiers statiques
builder.Services.Configure<StaticFileOptions>(options =>
{
    options.ServeUnknownFileTypes = true;
    options.DefaultContentType = "application/octet-stream";
});

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
app.UseCors();

// Servir les fichiers statiques depuis wwwroot
app.UseStaticFiles();

// Servir les images générées avec des en-têtes appropriés
app.UseStaticFiles( new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.WebRootPath, "images")),
    RequestPath = "/images",
    OnPrepareResponse = ctx =>
    {
        // Headers pour éviter le cache des images
        ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store");
        ctx.Context.Response.Headers.Append("Expires", "-1");
    }
});

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Créer le dossier images au démarrage
var imagesPath = Path.Combine(app.Environment.WebRootPath, "images");
Directory.CreateDirectory(imagesPath);

// Service de nettoyage en arrière-plan
app.Services.CreateScope().ServiceProvider
    .GetRequiredService<ICgmImageService>()
    .ClearOldCacheEntries(TimeSpan.FromHours(24));

// Lancer l'application
app.Run();