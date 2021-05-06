using Boiler.Api.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Boiler.Util.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate m_Next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            m_Next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await m_Next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = error switch
                {
                    ApiException            => (int)HttpStatusCode.BadRequest,
                    NotImplementedException => (int)HttpStatusCode.NotImplemented,
                    KeyNotFoundException    => (int)HttpStatusCode.NotFound,
                                          _ => response.StatusCode = (int)HttpStatusCode.InternalServerError,
                };
                var errors = new[] { error?.Message };
                var result = JsonSerializer.Serialize(new { errors = new { Exception = errors } });
                await response.WriteAsync(result);
            }
        }
    }
}
