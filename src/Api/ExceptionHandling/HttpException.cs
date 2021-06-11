using System;

namespace TicTacToeCSharpPlayground.Api.ExceptionHandling
{
    public abstract class HttpException : Exception
    {    
        public abstract int StatusCode { get; set; }
        public abstract string DefaultDetail { get; set; }
    }
}
