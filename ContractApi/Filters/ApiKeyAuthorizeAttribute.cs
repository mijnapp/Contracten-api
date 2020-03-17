using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ContractApi.Filters
{
    public class ApiKeyAuthorizeAttribute : Attribute, IAsyncActionFilter
    {
        public const string ApiKeyHeader = "Authorization";
        public const string ApiKeyConfiguration = "Api:OwnApiKey";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeader, out var potentialApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apikey = configuration.GetValue<string>(ApiKeyConfiguration);
            if (!apikey.Equals(potentialApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }
    }
}
