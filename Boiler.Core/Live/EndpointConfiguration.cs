using Boiler.Core.Live.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Boiler.Core.Live
{
    public static class EndpointConfiguration
    {
        public static void MapBoilerLive(this IEndpointRouteBuilder endpoint)
        {
            endpoint.MapHub<NotificationHub>("/notifications-hub");
        }
    }
}
