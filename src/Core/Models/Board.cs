using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TicTacToeCSharpPlayground.Core.Models
{
    public class Board : StandardEntity
    {
        public IList<Movement>? Movements { get; set; }
        public IList<PlayerBoard> PlayerBoards { get; set; }
        public int NumberOfColumn { get; set; }
        public int NumberOfRows { get; set; }
        [NotMapped] public IList<IList<Player?>> FieldsConfiguration { get; set; }
        [NotMapped] public IList<int> FreeFields { get; set; }

        public Player? GetRobotPlayer()
        {
            return PlayerBoards.FirstOrDefault(pb => !pb.Player.isNotComputer())?.Player;
        }

        public bool PositionIsNotAvailable(int movementPosition)
        {
            var copiedMovementPosition = movementPosition;
            return FreeFields.Any(position => position == copiedMovementPosition) is false;
        }

        public void InitializeBoardConfiguration()
        {
            var freeFields = new List<int>();
            var boardConfiguration = new List<IList<Player?>>();
            var positionCount = 1;

            for (int indexRow = 0; indexRow < NumberOfRows; indexRow++)
            {
                boardConfiguration.Add(new List<Player?>());
                for (int indexColumn = 0; indexColumn < NumberOfColumn; indexColumn++)
                {
                    var movement = Movements?.FirstOrDefault(m => m.Position == positionCount);

                    if (movement is not null)
                    {
                        boardConfiguration[indexRow].Add(movement.WhoMade);
                    }
                    else
                    {
                        boardConfiguration[indexRow].Add(null);
                        freeFields.Add(positionCount);
                    }

                    positionCount++;
                }
            }

            FieldsConfiguration = boardConfiguration;
            FreeFields = freeFields;
        }
    }
}
