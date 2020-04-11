using System.Collections.Generic;
using System.Linq;
using src.Helper;
using src.Repository;

namespace src.Business
{
    public interface IBoardJudge
    {
        public (int, int) GetRowAndColGivenAPosition(in int position, Board gameConfiguredBoard);
        bool WonHorizontally(IList<IList<Player?>> fields, Board gameConfiguredBoard, in int lastMovementPosition);
        bool WonVertically(IList<IList<Player?>> fields, Board gameConfiguredBoard, in int lastMovementPosition);
        bool WonDiagonally(IList<IList<Player?>> fields, Board gameConfiguredBoard, in int lastMovementPosition);
        bool DrawGame(IList<IList<Player?>> fields);
    }

    public class BoardJudge : IBoardJudge
    {
        public (int, int) GetRowAndColGivenAPosition(in int position, Board gameConfiguredBoard)
        {
            var refreshedMovementPosition = position - 1;

            var row = refreshedMovementPosition / gameConfiguredBoard.NumberOfRows;
            var col = refreshedMovementPosition % gameConfiguredBoard.NumberOfColumn;

            return (row, col);
        }

        public bool WonHorizontally(IList<IList<Player?>> fields, Board gameConfiguredBoard, in int lastMovementPosition)
        {
            throw new System.NotImplementedException();
        }

        public bool WonVertically(IList<IList<Player?>> fields, Board gameConfiguredBoard, in int lastMovementPosition)
        {
            throw new System.NotImplementedException();
        }

        public bool WonDiagonally(IList<IList<Player?>> fields, Board gameConfiguredBoard, in int lastMovementPosition)
        {
            throw new System.NotImplementedException();
        }

        public bool DrawGame(IList<IList<Player?>> fields)
        {
            return fields.All(row => row.All(col => col.IsNotNull()));
        }
    }
}
