using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Boiler.Util.Middleware
{
    public class ResponseHeaderMiddleware
    {
        private readonly RequestDelegate m_Next;

        public ResponseHeaderMiddleware(RequestDelegate next)
        {
            m_Next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            httpContext.Response.Headers.Add("ResponseTime", DateTime.UtcNow.ToString());
            httpContext.Response.Headers.Add("HostName", Environment.MachineName.ToString());
            await m_Next(httpContext);
        }
    }
}
