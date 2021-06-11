using System;

namespace TicTacToeCSharpPlayground.Api.Controllers.V1
{
    public class CreateBoardDto
    {
        public string? BoardSize { get; set; } 
        public long FirstPlayerId { get; set; }
        public long? SecondPlayerId { get; set; }
    }
}
