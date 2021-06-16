using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TicTacToeCSharpPlayground.Api.Controllers;

namespace TicTacToeCSharpPlayground.Api.Configs
{
    public class HttpExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; set; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // No need for it
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is HttpException exception)
            {
                context.Result = new ObjectResult(exception.Details)
                {
                    StatusCode = exception.StatusCode,
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
