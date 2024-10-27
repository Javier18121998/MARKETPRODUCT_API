using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Market.Exceptions.Middlewares
{
    public class MarketHandlingMiddleware
    {
        private readonly RequestDelegate _nextDelegate;
        private readonly ILogger<MarketHandlingMiddleware> _logger;
        public MarketHandlingMiddleware(RequestDelegate nextDelegate, ILogger<MarketHandlingMiddleware> logger)
        {
            _nextDelegate = nextDelegate;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _nextDelegate(context); 
            }
            catch (CustomException customEx)
            {
                await HandleCustomExceptionAsync(context, customEx);
            }
            catch (Exception ex)
            {
                await HandleUnexpectedExceptionAsync(context, ex);
            }
        }

        private Task HandleCustomExceptionAsync(HttpContext context, CustomException customEx)
        {
            _logger.LogError($"CustomException occurred: {customEx.Message} | Code: {customEx.ErrorCode}");
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)customEx.StatusCode;
            var errorCode = customEx.ErrorCode ?? "UnknownErrorCode";
            var stackTrace = customEx.StackTrace ?? "UnknownStackTrace";
            var customDetails = new
            {
                Status = (int)customEx.StatusCode,
                stackTrace = customEx.StackTrace,
                Detail = customEx.Message,
                ErrorCode = errorCode,
                InnerException = customEx.InnerException != null ? customEx.InnerException.Message : "No inner exception."
            };

            return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(customDetails));
        }

        private Task HandleUnexpectedExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError($"An unexpected error occurred: {ex.Message}");
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred. Please try again later."
            };

            return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(problemDetails));
        }
    }
}
