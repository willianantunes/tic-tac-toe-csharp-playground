using System;
using System.Collections.Generic;
using System.Linq;
using src.Helper;
using src.Repository;

namespace src.Business
{
    public interface IBoardJudge
    {
        public (int, int) GetRowAndColGivenAPosition(in int position, Board gameConfiguredBoard);
        bool WonHorizontally(Board gameConfiguredBoard, in int lastMovementPosition);
        bool WonVertically(Board gameConfiguredBoard, in int lastMovementPosition);
        bool WonDiagonally(Board gameConfiguredBoard, in int lastMovementPosition);
        bool WonReverseDiagonally(Board gameConfiguredBoard, in int lastMovementPosition);
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

        public bool WonHorizontally(Board gameConfiguredBoard, in int lastMovementPosition)
        {
            var (row, _) = GetRowAndColGivenAPosition(lastMovementPosition, gameConfiguredBoard);

            var fields = gameConfiguredBoard.FieldsConfiguration;
            var playerUsedToEvaluate = fields[row].First();
            Func<Player?, bool> predicate = p => p.IsNotNull() && p.Equals(playerUsedToEvaluate);
            var isPlayerPresentInAllHorizontalFields = fields[row].All(predicate);

            return isPlayerPresentInAllHorizontalFields;
        }

        public bool WonVertically(Board gameConfiguredBoard, in int lastMovementPosition)
        {
            var (row, col) = GetRowAndColGivenAPosition(lastMovementPosition, gameConfiguredBoard);

            var fields = gameConfiguredBoard.FieldsConfiguration;
            var playerUsedToEvaluate = fields[row][col];
            Func<IList<Player?>, bool> predicate = row => row[col].IsNotNull() && row[col].Equals(playerUsedToEvaluate);
            var isPlayerPresentInAllVerticalFields = fields.All(predicate);

            return isPlayerPresentInAllVerticalFields;
        }

        public bool WonDiagonally(Board gameConfiguredBoard, in int lastMovementPosition)
        {
            var (row, col) = GetRowAndColGivenAPosition(lastMovementPosition, gameConfiguredBoard);

            if (row != col)
                return false;

            var fields = gameConfiguredBoard.FieldsConfiguration;
            var playerUsedToEvaluate = fields[row][col];
            var columnCounter = 0;

            return fields.All(slice => slice[columnCounter++].Equals(playerUsedToEvaluate));
        }

        public bool WonReverseDiagonally(Board gameConfiguredBoard, in int lastMovementPosition)
        {
            var (row, col) = GetRowAndColGivenAPosition(lastMovementPosition, gameConfiguredBoard);

            var isNotEligible = (row + col) != (gameConfiguredBoard.NumberOfRows - 1);

            if (isNotEligible)
                return false;

            var fields = gameConfiguredBoard.FieldsConfiguration;
            var playerUsedToEvaluate = fields[row][col];
            var columnCounter = fields[row].Count;

            return fields.All(slice => slice[columnCounter--].Equals(playerUsedToEvaluate));
        }

        public bool DrawGame(IList<IList<Player?>> fields)
        {
            return fields.All(row => row.All(col => col.IsNotNull()));
        }
    }
}
