using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using src.Helper;
using src.Repository;

namespace src.Business
{
    public interface IBoardDealer
    {
        bool NotValidOrUnsupportedBoardSize(string? boardSize);
        Task<Board> CreateNewBoard(string? boardSize, Player playerOne, Player playerTwo = null);
        void InitializeBoardConfiguration(Board board);
        bool PositionIsNotAvailable(Board gameConfiguredBoard, in int movementPosition);
        IList<int> AvailablePositions(Board gameConfiguredBoard);
        void ApplyMovement(Board gameConfiguredBoard, in int movementPosition, Player player);
        BoardSituation EvaluateTheSituation(Board gameConfiguredBoard, in int lastMovementPosition);
        bool HasComputerPlayer(Board gameConfiguredBoard);
        void ApplyMovementForComputer(Board gameConfiguredBoard, in int somePosition);
    }

    public class BoardDealer : IBoardDealer
    {
        private IBoardJudge _boardJudge;
        private Regex _almostValidBoardSetup = new Regex(@"[3-9]x[3-9]");

        public bool NotValidOrUnsupportedBoardSize(string? boardSize)
        {
            if (boardSize.IsNull() || _almostValidBoardSetup.Match(boardSize).NotSuccess())
                return true;

            var column = boardSize.Substring(0, 1);
            var rows = boardSize.Substring(2, 1);

            return column != rows;
        }

        public Task<Board> CreateNewBoard(string? boardSize, Player playerOne, Player playerTwo = null)
        {
            throw new System.NotImplementedException();
        }

        public void InitializeBoardConfiguration(Board board)
        {
            var freeFields = new List<int>();
            var boardConfiguration = new List<IList<Player?>>();
            var positionCount = 1;

            for (int indexRow = 0; indexRow < board.NumberOfRows; indexRow++)
            {
                boardConfiguration.Add(new List<Player?>());
                for (int indexColumn = 0; indexColumn < board.NumberOfColumn; indexColumn++)
                {
                    var movement = board.Movements.FirstOrDefault(m => m.Position == positionCount);

                    if (movement.IsNotNull())
                        boardConfiguration[indexRow].Add(movement.WhoMade);
                    else
                    {
                        boardConfiguration[indexRow].Add(null);
                        freeFields.Add(positionCount);
                    }

                    positionCount++;
                }
            }

            board.FieldsConfiguration = boardConfiguration;
            board.FreeFields = freeFields;
        }

        public bool PositionIsNotAvailable(Board board, in int movementPosition)
        {
            var copiedMovementPosition = movementPosition;
            return board.FreeFields.Any(position => position == copiedMovementPosition);
        }

        public IList<int> AvailablePositions(Board board)
        {
            return board.FreeFields;
        }

        public void ApplyMovement(Board gameConfiguredBoard, in int movementPosition, Player player)
        {
            var (row, col) = _boardJudge.GetRowAndColGivenAPosition(movementPosition, gameConfiguredBoard);

            gameConfiguredBoard.FieldsConfiguration[row][col] = player;
            // TODO raise exception if remove action returns false
            gameConfiguredBoard.FreeFields.Remove(movementPosition);
        }

        public BoardSituation EvaluateTheSituation(Board gameConfiguredBoard, in int lastMovementPosition)
        {
            var fields = gameConfiguredBoard.FieldsConfiguration;

            bool wonHorizontally = _boardJudge.WonHorizontally(fields, gameConfiguredBoard, lastMovementPosition);
            bool wonVertically = _boardJudge.WonVertically(fields, gameConfiguredBoard, lastMovementPosition);
            bool wonDiagonally = _boardJudge.WonDiagonally(fields, gameConfiguredBoard, lastMovementPosition);

            var boardSituation = new BoardSituation();
            
            boardSituation.HasAWinner = wonHorizontally || wonVertically || wonDiagonally;
            if (boardSituation.HasAWinner)
                boardSituation.Winner = new Player();
            else
            {
                bool drawGame = _boardJudge.DrawGame(fields);
                if (drawGame)
                    boardSituation.SadlyFinishedWithDraw = true;
            }

            return boardSituation;
        }

        public bool HasComputerPlayer(Board gameConfiguredBoard)
        {
            return gameConfiguredBoard.PlayerBoards.Any(pb => !pb.Player.isNotComputer());
        }

        public void ApplyMovementForComputer(Board gameConfiguredBoard, in int somePosition)
        {
            var (row, col) = _boardJudge.GetRowAndColGivenAPosition(somePosition, gameConfiguredBoard);
            var pBoard = gameConfiguredBoard.PlayerBoards.First(pb => !pb.Player.isNotComputer());
            gameConfiguredBoard.FieldsConfiguration[row][col] = pBoard.Player;
            // TODO raise exception if remove action returns false
            gameConfiguredBoard.FreeFields.Remove(somePosition);
        }
    }

    public class BoardSituation
    {
        public bool SadlyFinishedWithDraw { get; set; }
        public bool HasAWinner { get; set; }
        public Player Winner { get; set; }
        public bool Concluded { get; set; }
    }
}
