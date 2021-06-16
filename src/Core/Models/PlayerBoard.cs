namespace TicTacToeCSharpPlayground.Core.Models
{
    public class PlayerBoard : StandardEntity
    {
        public int PlayerId { get; set; }
        public Player Player { get; set; }
        public int BoardId { get; set; }
        public Board Board { get; set; }
    }
}
