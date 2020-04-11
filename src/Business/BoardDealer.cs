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
        BoardSituation EvaluateTheSituation(Board gameConfiguredBoard);
        bool HasComputerPlayer(Board gameConfiguredBoard);
        void ApplyMovementForComputer(Board gameConfiguredBoard, in int somePosition);
    }

    public class BoardDealer : IBoardDealer
    {
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
            throw new System.NotImplementedException();
        }

        public BoardSituation EvaluateTheSituation(Board gameConfiguredBoard)
        {
            throw new System.NotImplementedException();
        }

        public bool HasComputerPlayer(Board gameConfiguredBoard)
        {
            throw new System.NotImplementedException();
        }

        public void ApplyMovementForComputer(Board gameConfiguredBoard, in int somePosition)
        {
            throw new System.NotImplementedException();
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