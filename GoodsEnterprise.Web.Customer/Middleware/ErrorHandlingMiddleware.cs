using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Middleware
{
    /// <summary>
    /// ErrorHandlingMiddleware
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        /// <summary>
        /// ErrorHandlingMiddleware
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        public ErrorHandlingMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Middleware Exception");
                throw;
            }
        }
    }
}
