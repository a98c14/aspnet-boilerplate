using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Boiler.Live.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Boiler.Live.Extensions
{
    public static class EndpointExtensions
    {
        public static void MapBoilerLive(this IEndpointRouteBuilder endpoint)
        {
            endpoint.MapHub<NotificationHub>("/notifications-hub");
        }
    }
}
