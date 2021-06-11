namespace TicTacToeCSharpPlayground.Core.Models
{
    public class Game : StandardEntity
    {
        public Player Winner { get; set; }
        public bool Draw { get; set; }
        public bool Finished { get; set; }
        public Board ConfiguredBoard { get; set; }

        public Game()
        {
            
        }

        public Game(Board board)
        {
            Finished = false;
            ConfiguredBoard = board;
        }

        public bool IsFinished()
        {
            return Finished;
        }
    }
}
