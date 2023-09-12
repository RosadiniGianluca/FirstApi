using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FirstApi.Authentication
{
    public class ApiKeyAuthenticationFilter : IAuthorizationFilter
    {
        private readonly IConfiguration _configuration;

        public ApiKeyAuthenticationFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if(!context.HttpContext.Request.Headers.TryGetValue(AuthenticationCostants.ApiKeyHeaderName, out
                                   var extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult("Api Key is Missing");
                return;
            }

            var apiKey = _configuration.GetValue<string>(AuthenticationCostants.ApiKeySectionName);
            if(!apiKey.Equals(extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult("Invalid Api Key");
                return;
            }
            
        }
    }
}