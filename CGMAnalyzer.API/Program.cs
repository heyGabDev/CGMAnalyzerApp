using CGMAnalyzer.API.Services;
using CGMAnalyzer.API.Services.Interfaces;
using CGMAnalyzerCore.Convert;
using CGMAnalyzerCore.Converter.Interface;
using CGMAnalyzerCore.Exporter;
using Microsoft.Extensions.FileProviders;

// L'ordre du pipeline ASP.NET Core est important :
// CORS avant UseStaticFiles
// UseRouting avant UseAuthorization

var builder = WebApplication.CreateBuilder(args);

// Configuration des services de base
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "CGM Analyzer API", Version = "v1" });
});

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

// Configuration du logging 
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Services métier CGM
builder.Services.AddScoped<ICgmConverter, CgmConverter>();
builder.Services.AddScoped<ICgmImageService, CgmImageService>();
builder.Services.AddScoped<CgmExportService>();
builder.Services.AddScoped<ICGMLayerDetector, CGMLayerDetector>();

// Configuration hébergement
builder.WebHost.UseUrls("https://localhost:7176");
// ? pour que WebRootPath fonctionne :
builder.WebHost.UseWebRoot("wwwroot");

var app = builder.Build();
// Pipeline de développement
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CGM Analyzer API v1");
        c.RoutePrefix = "swagger";
    });
    app.UseDeveloperExceptionPage();
}

// Middleware HTTP
app.UseHttpsRedirection();
app.UseCors();

// Servir les fichiers statiques depuis wwwroot
app.UseStaticFiles();

// Servir les images générées avec configuration spéciale
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.WebRootPath, "images")),
    RequestPath = "/images",
    OnPrepareResponse = ctx =>
    {
        // Éviter la mise en cache des images pour voir les changements immédiatement
        ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
        ctx.Context.Response.Headers.Append("Pragma", "no-cache");
        ctx.Context.Response.Headers.Append("Expires", "0");
    }
});

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Initialisation au démarrage
await InitializeApplicationAsync(app);

app.Run();

// Méthode d'initialisation (séparée = clarté)
static async Task InitializeApplicationAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Créer le dossier images
        var imagesPath = Path.Combine(app.Environment.WebRootPath, "images");
        Directory.CreateDirectory(imagesPath);
        logger.LogInformation("Dossier images créé/vérifié: {ImagesPath}", imagesPath);

        // Nettoyage initial du cache (si le service avancé existe)
        var imageService = scope.ServiceProvider.GetRequiredService<ICgmImageService>();
        if (imageService is CgmImageService advancedService)
        {
            advancedService.ClearOldCacheEntries(TimeSpan.FromHours(24));
            logger.LogInformation("Cache initial nettoyé");
        }

        logger.LogInformation("Application CGM Analyzer API initialisée avec succès");
        logger.LogInformation("Swagger UI disponible sur: https://localhost:7176/swagger");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erreur lors de l'initialisation de l'application");
        throw;
    }
}


//// Configuration des fichiers statiques
//builder.Services.Configure<StaticFileOptions>(options =>
//{
//    options.ServeUnknownFileTypes = true;
//    options.DefaultContentType = "application/octet-stream";
//});


//// Activer Swagger uniquement en développement
//if (app.Environment.IsDevelopment())
//{
//    app.UseStaticFiles();
//    app.UseSwagger();
//    app.UseSwaggerUI();
//    app.UseDeveloperExceptionPage();
//}

//// Middleware HTTP
//app.UseHttpsRedirection();
//app.UseAuthorization();
//app.UseCors();

//// Servir les fichiers statiques depuis wwwroot
//app.UseStaticFiles();

//// Servir les images générées avec des en-têtes appropriés
//app.UseStaticFiles( new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(
//        Path.Combine(app.Environment.WebRootPath, "images")),
//    RequestPath = "/images",
//    OnPrepareResponse = ctx =>
//    {
//        // Headers pour éviter le cache des images
//        ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store");
//        ctx.Context.Response.Headers.Append("Expires", "-1");
//    }
//});

//app.UseRouting();
//app.UseAuthorization();
//app.MapControllers();

//// Créer le dossier images au démarrage
//var imagesPath = Path.Combine(app.Environment.WebRootPath, "images");
//Directory.CreateDirectory(imagesPath);

//// Service de nettoyage en arrière-plan
//app.Services.CreateScope().ServiceProvider
//    .GetRequiredService<ICgmImageService>()
//    .ClearOldCacheEntries(TimeSpan.FromHours(24));

//// Lancer l'application
//app.Run();