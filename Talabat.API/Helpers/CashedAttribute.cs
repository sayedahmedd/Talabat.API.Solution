using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Talabat.Core.Services.Contract;

namespace Talabat.API.Helpers
{
    public class CashedAttribute : Attribute , IAsyncActionFilter
    {
        private readonly int _timeToLiveInSeconds;

        public CashedAttribute(int timeToLiveInSeconds)
        {
            _timeToLiveInSeconds = timeToLiveInSeconds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var responseCasheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCasheService>();
            //Ask clr for create obj from "ResponseCasheService" Explicitly
            var casheKey = GenerateCasheKeyFromRequest(context.HttpContext.Request);
            var response = await responseCasheService.GetCashedResponseAsync(casheKey);
            if(!string.IsNullOrEmpty(response))
            {
                var result = new ContentResult()
                {
                    Content = response,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = result;
                return;
            }
            var executedActionContext = await next.Invoke(); // will execute next action filter or action itself
            if(executedActionContext.Result is OkObjectResult okObjectResult && okObjectResult.Value is not null) 
            {
                await responseCasheService.CasheResponseAsync(casheKey,okObjectResult.Value,TimeSpan.FromSeconds(_timeToLiveInSeconds));
            }
        }

        private string GenerateCasheKeyFromRequest(HttpRequest request)
        {
            //{{url}}/api/products?pageIndex=1&pageSize=5&sort=name
            var keyBuilder = new StringBuilder();
            keyBuilder.Append(request.Path); //api/products
            //pageIndex=1
            //pageSize=5
            //sort=name
            foreach(var (key, value) in request.Query.OrderBy(x => x.Key)) 
            {
                keyBuilder.Append($"|{key}-{value}");
            }
            return keyBuilder.ToString();
        }
    }
}
