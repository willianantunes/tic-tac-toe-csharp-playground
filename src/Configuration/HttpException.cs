using System;

namespace src.Configuration
{
    public abstract class HttpException : Exception
    {    
        public abstract int StatusCode { get; set; }
        public abstract string DefaultDetail { get; set; }
    }
}
