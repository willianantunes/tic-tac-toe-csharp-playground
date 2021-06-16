using System;
using System.Collections.Generic;
using System.Linq;
using TicTacToeCSharpPlayground.Core.Models;

namespace TicTacToeCSharpPlayground.Core.Business
{
    public interface IBoardJudge
    {
        (int, int) GetRowAndColGivenAPosition(in int position, Board gameConfiguredBoard);
        bool WonHorizontally(Board gameConfiguredBoard, in int lastMovementPosition);
        bool WonVertically(Board gameConfiguredBoard, in int lastMovementPosition);
        bool WonDiagonally(Board gameConfiguredBoard, in int lastMovementPosition);
        bool WonReverseDiagonally(Board gameConfiguredBoard, in int lastMovementPosition);
        bool DrawGame(IList<IList<Player?>> fields);
    }

    public class BoardJudge : IBoardJudge
    {
        public (int, int) GetRowAndColGivenAPosition(in int position, Board board)
        {
            var refreshedMovementPosition = position - 1;

            var row = refreshedMovementPosition / board.NumberOfRows;
            var col = refreshedMovementPosition % board.NumberOfColumn;

            return (row, col);
        }

        public bool WonHorizontally(Board gameConfiguredBoard, in int lastMovementPosition)
        {
            var (row, _) = GetRowAndColGivenAPosition(lastMovementPosition, gameConfiguredBoard);

            var fields = gameConfiguredBoard.FieldsConfiguration;
            var playerUsedToEvaluate = fields[row].First();
            Func<Player?, bool> predicate = p => p is not null && p.Equals(playerUsedToEvaluate);
            var isPlayerPresentInAllHorizontalFields = fields[row].All(predicate);

            return isPlayerPresentInAllHorizontalFields;
        }

        public bool WonVertically(Board gameConfiguredBoard, in int lastMovementPosition)
        {
            var (row, col) = GetRowAndColGivenAPosition(lastMovementPosition, gameConfiguredBoard);

            var fields = gameConfiguredBoard.FieldsConfiguration;
            var playerUsedToEvaluate = fields[row][col];
            Func<IList<Player?>, bool> predicate = row => row[col] is not null && row[col].Equals(playerUsedToEvaluate);
            var isPlayerPresentInAllVerticalFields = fields.All(predicate);

            return isPlayerPresentInAllVerticalFields;
        }

        public bool WonDiagonally(Board gameConfiguredBoard, in int lastMovementPosition)
        {
            var (row, col) = GetRowAndColGivenAPosition(lastMovementPosition, gameConfiguredBoard);

            if (row != col)
                return false;

            var fields = gameConfiguredBoard.FieldsConfiguration;
            var foundPlayer = fields[row][col];
            var columnCounter = 0;
            Func<IList<Player?>, bool> predicate = row =>
            {
                var maybeAPlayerHere = row[columnCounter++];
                return maybeAPlayerHere is not null && maybeAPlayerHere.Equals(foundPlayer);
            };

            return fields.All(predicate);
        }

        public bool WonReverseDiagonally(Board gameConfiguredBoard, in int lastMovementPosition)
        {
            var (row, col) = GetRowAndColGivenAPosition(lastMovementPosition, gameConfiguredBoard);

            var isNotEligible = (row + col) != (gameConfiguredBoard.NumberOfRows - 1);

            if (isNotEligible)
                return false;

            var fields = gameConfiguredBoard.FieldsConfiguration;
            var foundPlayer = fields[row][col];
            var columnCounter = fields[row].Count - 1;

            bool Predicate(IList<Player?> row)
            {
                var maybeAPlayerHere = row[columnCounter--];
                return maybeAPlayerHere is not null && maybeAPlayerHere.Equals(foundPlayer);
            }

            return fields.All(Predicate);
        }

        public bool DrawGame(IList<IList<Player?>> fields)
        {
            return fields.All(row => row.All(col => col is not null));
        }
    }
}
