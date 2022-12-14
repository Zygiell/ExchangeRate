using ExchangeRateAPI.Exceptions;
using Newtonsoft.Json;

namespace ExchangeRateAPI.Middlewares
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (NotFoundException notFoundException)
            {
                _logger.LogError($"Not found exception, no exchange rates from future");
                context.Response.StatusCode = 404;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject("Something went wrong"));
            }
        }
    }
}