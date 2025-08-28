namespace CGMAnalyzer.API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur non gérée dans la requête {RequestPath}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse
            {
                Message = "Une erreur interne s'est produite",
                Timestamp = DateTime.UtcNow
            };

            switch (exception)
            {
                case FileNotFoundException:
                    context.Response.StatusCode = 404;
                    response.Message = "Fichier non trouvé";
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = 403;
                    response.Message = "Accès interdit";
                    break;

                case ArgumentException:
                case InvalidOperationException:
                    context.Response.StatusCode = 400;
                    response.Message = "Requête invalide";
                    response.Details = exception.Message;
                    break;

                default:
                    context.Response.StatusCode = 500;
                    break;
            }

            var jsonResponse = System.Text.Json.JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }

    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
