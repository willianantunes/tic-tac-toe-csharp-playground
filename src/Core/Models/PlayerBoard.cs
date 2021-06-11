namespace TicTacToeCSharpPlayground.Core.Models
{
    public class PlayerBoard : StandardEntity
    {
        public long PlayerId { get; set; }
        public Player Player { get; set; }
        public long BoardId { get; set; }
        public Board Board { get; set; }
    }
}
