using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
                _logger.LogError(ex, "An unhandled exception occurred. Request: {Method} {Path}", 
                    context.Request.Method, context.Request.Path);
                
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse();

            switch (exception)
            {
                case ArgumentNullException _:
                case ArgumentException _:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = "Invalid request parameters.";
                    break;

                case UnauthorizedAccessException _:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse.Message = "Access denied. Please login to continue.";
                    break;

                case KeyNotFoundException _:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.Message = "The requested resource was not found.";
                    break;

                case TimeoutException _:
                    response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                    errorResponse.Message = "The request timed out. Please try again.";
                    break;

                case InvalidOperationException _:
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    errorResponse.Message = "The operation could not be completed due to a conflict.";
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Message = "An unexpected error occurred. Please try again later.";
                    break;
            }

            // Add request ID for tracking
            errorResponse.RequestId = context.TraceIdentifier;
            errorResponse.Timestamp = DateTime.UtcNow;

            // In development, include stack trace
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                errorResponse.Details = exception.Message;
                errorResponse.StackTrace = exception.StackTrace;
            }

            // For web requests, redirect to error page
            if (context.Request.Headers["Accept"].ToString().Contains("text/html"))
            {
                // Store error details in session if available
                try
                {
                    if (context.Session != null)
                    {
                        context.Session.SetString("ErrorDetails", JsonConvert.SerializeObject(errorResponse));
                    }
                }
                catch (InvalidOperationException)
                {
                    // Session not available, continue without storing details
                }
                
                response.Redirect("/Error");
                return;
            }

            // For API requests, return JSON
            var jsonResponse = JsonConvert.SerializeObject(errorResponse, Formatting.Indented);
            await response.WriteAsync(jsonResponse);
        }
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
        public string RequestId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Details { get; set; }
        public string StackTrace { get; set; }
    }
}
