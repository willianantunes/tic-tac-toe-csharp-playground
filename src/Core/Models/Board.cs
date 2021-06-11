using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicTacToeCSharpPlayground.Core.Models
{
    public class Board : StandardEntity
    {
        public IList<Movement> Movements { get; set; }
        public IList<PlayerBoard> PlayerBoards { get; set; }
        public int NumberOfColumn { get; set; }
        public int NumberOfRows { get; set; }
        [NotMapped] public IList<IList<Player?>> FieldsConfiguration { get; set; }
        [NotMapped] public IList<int> FreeFields { get; set; }
    }
}
