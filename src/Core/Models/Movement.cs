namespace TicTacToeCSharpPlayground.Core.Models
{
    public class Movement : StandardEntity
    {
        public int Position { get; set; }
        public int BoardId { get; set; }
        public Board Board { get; set; }
        public Player WhoMade { get; set; }
    }
}
