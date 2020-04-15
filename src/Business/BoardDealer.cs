using System;
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
        Task<Board> CreateNewBoard(string boardSize, Player playerOne, Player playerTwo);
        void InitializeBoardConfiguration(Board board);
        bool PositionIsNotAvailable(Board gameConfiguredBoard, in int movementPosition);
        IList<int> AvailablePositions(Board gameConfiguredBoard);
        Task<Board> ApplyMovement(Board board, int movementPosition, Player player);
        BoardSituation EvaluateTheSituation(Board gameConfiguredBoard, in int lastMovementPosition);
        bool HasComputerPlayer(Board gameConfiguredBoard);
        Task ApplyMovementForComputer(Board board, int movementPosition);
    }

    public class BoardDealer : IBoardDealer
    {
        private IBoardJudge _boardJudge;
        private readonly ITicTacToeRepository _ticTacToeRepository;
        private Regex _almostValidBoardSetup = new Regex(@"[3-9]x[3-9]");

        public BoardDealer()
        {
        }

        public BoardDealer(IBoardJudge boardJudge, ITicTacToeRepository ticTacToeRepository)
        {
            _boardJudge = boardJudge;
            _ticTacToeRepository = ticTacToeRepository;
        }

        public bool NotValidOrUnsupportedBoardSize(string? boardSize)
        {
            if (boardSize.IsNull() || _almostValidBoardSetup.Match(boardSize).NotSuccess())
                return true;

            var column = boardSize.Substring(0, 1);
            var rows = boardSize.Substring(2, 1);

            return column != rows;
        }

        public async Task<Board> CreateNewBoard(string boardSize, Player playerOne, Player playerTwo)
        {
            var column = boardSize.Substring(0, 1).ToInt();
            var rows = boardSize.Substring(2, 1).ToInt();

            var board = new Board {NumberOfColumn = column, NumberOfRows = rows};
            var playerBoardOne = new PlayerBoard{Player = playerOne, Board = board};
            var playerBoarTwo = new PlayerBoard{Player = playerTwo, Board = board};
            board.PlayerBoards = new List<PlayerBoard>{playerBoardOne, playerBoarTwo};
            
            await _ticTacToeRepository.SaveBoard(board);

            return board;
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
                    Func<Movement, bool> predicate = m => m.Position == positionCount;
                    var movement = board.Movements?.FirstOrDefault(predicate);

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
            return board.FreeFields.None(position => position == copiedMovementPosition);
        }

        public IList<int> AvailablePositions(Board board)
        {
            return board.FreeFields;
        }

        public async Task<Board> ApplyMovement(Board board, int movementPosition, Player player)
        {
            var (row, col) = _boardJudge.GetRowAndColGivenAPosition(movementPosition, board);

            board.FieldsConfiguration[row][col] = player;
            // TODO raise exception if remove action returns false
            board.FreeFields.Remove(movementPosition);

            var movement = new Movement{Position = movementPosition, WhoMade = player};

            return await _ticTacToeRepository.CreateMovementAndRefreshBoard(movement, board);
        }

        public BoardSituation EvaluateTheSituation(Board gameConfiguredBoard, in int lastMovementPosition)
        {
            bool wonHorizontally = _boardJudge.WonHorizontally(gameConfiguredBoard, lastMovementPosition);
            bool wonVertically = _boardJudge.WonVertically(gameConfiguredBoard, lastMovementPosition);
            bool wonDiagonally = _boardJudge.WonDiagonally(gameConfiguredBoard, lastMovementPosition);
            bool wonReverseDiagonally = _boardJudge.WonReverseDiagonally(gameConfiguredBoard, lastMovementPosition);

            var boardSituation = new BoardSituation();

            boardSituation.HasAWinner = wonHorizontally || wonVertically || wonDiagonally || wonReverseDiagonally;
            if (boardSituation.HasAWinner)
                return boardSituation;
            
            var fields = gameConfiguredBoard.FieldsConfiguration;
            bool drawGame = _boardJudge.DrawGame(fields);
            if (drawGame)
                boardSituation.SadlyFinishedWithDraw = true;

            return boardSituation;
        }

        public bool HasComputerPlayer(Board gameConfiguredBoard) 
        {
            return gameConfiguredBoard.PlayerBoards.Any(pb => !pb.Player.isNotComputer());
        }

        public async Task ApplyMovementForComputer(Board board, int movementPosition)
        {
            var pBoard = board.PlayerBoards.First(pb => !pb.Player.isNotComputer());

            await ApplyMovement(board, movementPosition, pBoard.Player);
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
