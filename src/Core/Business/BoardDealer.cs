using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using TicTacToeCSharpPlayground.Core.Models;

namespace TicTacToeCSharpPlayground.Core.Business
{
    public interface IBoardDealer
    {
        bool NotValidOrUnsupportedBoardSize(string boardSize);
        Board PrepareBoardWithRequestSetup(string boardSize, Player playerOne, Player playerTwo);
        Movement CreateMovementForCustomPlayerOrComputer(Board board, int position, Player? player = null);
        BoardState EvaluateTheSituation(Board board, in int lastMovementPosition);
    }

    public class BoardDealer : IBoardDealer
    {
        private IBoardJudge _boardJudge;
        private Regex _almostValidBoardSetup = new(@"[3-9]x[3-9]");

        public BoardDealer(IBoardJudge boardJudge)
        {
            _boardJudge = boardJudge;
        }

        public bool NotValidOrUnsupportedBoardSize(string boardSize)
        {
            if (_almostValidBoardSetup.Match(boardSize).Success is false)
                return true;

            var column = boardSize.Substring(0, 1);
            var rows = boardSize.Substring(2, 1);

            return column != rows;
        }

        public Board PrepareBoardWithRequestSetup(string boardSize, Player playerOne, Player playerTwo)
        {
            var column = int.Parse(boardSize.Substring(0, 1));
            var rows = int.Parse(boardSize.Substring(2, 1));

            var board = new Board { NumberOfColumn = column, NumberOfRows = rows };
            var playerBoardOne = new PlayerBoard { Player = playerOne, Board = board };
            var playerBoarTwo = new PlayerBoard { Player = playerTwo, Board = board };
            board.PlayerBoards = new List<PlayerBoard> { playerBoardOne, playerBoarTwo };
            board.InitializeBoardConfiguration();

            return board;
        }

        public Movement CreateMovementForCustomPlayerOrComputer(Board board, int position, Player? player = null)
        {
            player ??= board.PlayerBoards.First(pb => !pb.Player.isNotComputer()).Player;

            var (row, col) = _boardJudge.GetRowAndColGivenAPosition(position, board);

            board.FieldsConfiguration[row][col] = player;
            var movementWasRemoved = board.FreeFields.Remove(position);
            Trace.Assert(movementWasRemoved is true);

            return new Movement { Position = position, WhoMade = player };
        }

        public BoardState EvaluateTheSituation(Board board, in int lastMovementPosition)
        {
            bool wonHorizontally = _boardJudge.WonHorizontally(board, lastMovementPosition);
            bool wonVertically = _boardJudge.WonVertically(board, lastMovementPosition);
            bool wonDiagonally = _boardJudge.WonDiagonally(board, lastMovementPosition);
            bool wonReverseDiagonally = _boardJudge.WonReverseDiagonally(board, lastMovementPosition);

            bool hasAWinner = wonHorizontally || wonVertically || wonDiagonally || wonReverseDiagonally;
            var fields = board.FieldsConfiguration;
            bool drawGame = _boardJudge.DrawGame(fields);

            var possibleConditionOne = hasAWinner is true && drawGame is false;
            var possibleConditionTwo = hasAWinner is false && drawGame is true;
            var possibleConditionThree = hasAWinner is false && drawGame is false;
            Trace.Assert(possibleConditionOne || possibleConditionTwo || possibleConditionThree);

            return new BoardState(hasAWinner, drawGame);
        }
    }
}
