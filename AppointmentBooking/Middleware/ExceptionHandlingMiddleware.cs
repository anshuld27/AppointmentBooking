using System.Text.Json;

namespace AppointmentBooking.Middleware
{
    /// <summary>
    /// Middleware for handling exceptions globally in the application pipeline
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        // The next middleware component in the pipeline
        private readonly RequestDelegate _next;

        // Logger for recording exception information
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the ExceptionHandlingMiddleware
        /// </summary>
        /// <param name="next">Next middleware in the request pipeline</param>
        /// <param name="logger">Logger instance for error logging</param>
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware to process HTTP requests
        /// </summary>
        /// <param name="context">The current HTTP context</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Pass the request to the next middleware component
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the exception details
                _logger.LogError(ex, "An error occurred");

                // Handle the exception and generate appropriate response
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handles exceptions and generates standardized error responses
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="exception">The caught exception</param>
        /// <returns>JSON-formatted error response</returns>
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Set response content type to JSON
            context.Response.ContentType = "application/json";

            // Determine HTTP status code based on exception type
            context.Response.StatusCode = exception switch
            {
                ArgumentException => StatusCodes.Status400BadRequest,  // Client-side error
                _ => StatusCodes.Status500InternalServerError          // All other exceptions
            };

            // Create and serialize error response object
            return context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                message = exception.Message  // Return exception message to client
            }));
        }
    }
}