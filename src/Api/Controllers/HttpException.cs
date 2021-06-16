using System;

namespace TicTacToeCSharpPlayground.Api.Controllers
{
    public class HttpException : Exception
    {
        public int StatusCode { get; set; }
        public string Details { get; set; }
    }
}
