namespace TicTacToeCSharpPlayground.Core.Models
{
    public class Game : StandardEntity
    {
        public Player? Winner { get; set; }
        public bool Draw { get; set; }
        public bool Finished { get; set; }
        public Board? ConfiguredBoard { get; set; }

        public Game()
        {
            // Bodyless constructor. Needed by EF!
        }

        public Game(Board board)
        {
            Finished = false;
            board.InitializeBoardConfiguration();
            ConfiguredBoard = board;
        }

        public bool IsFinished()
        {
            return Finished;
        }
    }
}
