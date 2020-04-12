using System;

namespace src.Controllers
{
    public class CreateBoardDto
    {
        public string? BoardSize { get; set; } 
        public Guid FirstPlayerId { get; set; }
        public Guid? SecondPlayerId { get; set; }
    }
}
