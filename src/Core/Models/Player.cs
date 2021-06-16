using System.Collections.Generic;

namespace TicTacToeCSharpPlayground.Core.Models
{
    public class Player : StandardEntity
    {
        public IList<PlayerBoard>? PlayerBoards { get; set; }
        public string Name { get; set; }
        public bool Computer { get; set; }

        public bool isNotComputer()
        {
            return !Computer;
        }
    }
}
