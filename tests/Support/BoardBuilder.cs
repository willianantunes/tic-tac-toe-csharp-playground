using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicTacToeCSharpPlayground.Core.Business;
using TicTacToeCSharpPlayground.Core.Models;
using TicTacToeCSharpPlayground.Infrastructure.Database;

namespace Tests.Support
{
    public class BoardBuilder
    {
        private int _boardSize;
        private Player _player;
        private int _rowToStartFilling = -1;
        private int _fillUntilColumn = -1;
        private int _startFromRow = -1;
        private int _defaultColumn = -1;
        private int _fillUntilRow = -1;
        private int _fillDiagonallyUntilRow = -1;
        private int _fillReverseDiagonallyUntilRow = -1;

        public BoardBuilder BoardSize(int boardSize)
        {
            _boardSize = 3;
            return this;
        }

        public BoardBuilder WithPlayer(Player player)
        {
            _player = player;
            return this;
        }

        public BoardBuilder GivenRow(int rowToStartFilling)
        {
            _rowToStartFilling = rowToStartFilling;
            return this;
        }

        public BoardBuilder FillAllColumnsUntilColumn(Int16 fillUntilColumn)
        {
            _fillUntilColumn = fillUntilColumn;
            return this;
        }

        public BoardBuilder GivenRowAndColumn(int row, int col)
        {
            _startFromRow = row;
            _defaultColumn = col;
            return this;
        }

        public BoardBuilder FillAllRowsUntilRows(int fillUntilRow)
        {
            _fillUntilRow = fillUntilRow;
            return this;
        }

        public BoardBuilder FillDiagonallyUntilRow(int row)
        {
            _fillDiagonallyUntilRow = row;
            return this;
        }

        public BoardBuilder FillReverseDiagonallyUntilRow(int row)
        {
            _fillReverseDiagonallyUntilRow = row;
            return this;
        }

        public Board Build()
        {
            var board = new Board();
            board.Movements = new List<Movement>();
            board.NumberOfRows = _boardSize;
            board.NumberOfColumn = _boardSize;
            var boardDealer = new BoardDealer();
            boardDealer.InitializeBoardConfiguration(board);
            var fields = board.FieldsConfiguration;

            if (_fillReverseDiagonallyUntilRow >= 0)
            {
                for (var row = _startFromRow; row <= _fillReverseDiagonallyUntilRow; row++)
                    fields[row][_defaultColumn--] = _player;

                return board;
            }

            if (_fillDiagonallyUntilRow >= 0)
            {
                for (var row = _startFromRow; row <= _fillDiagonallyUntilRow; row++)
                    fields[row][_defaultColumn++] = _player;

                return board;
            }

            if (_fillUntilRow >= 0)
            {
                for (var row = _startFromRow; row <= _fillUntilRow; row++)
                    fields[row][_defaultColumn] = _player;

                return board;
            }

            if (_fillUntilColumn >= 0)
            {
                for (var column = 0; column <= _fillUntilColumn; column++)
                    fields[_rowToStartFilling][column] = _player;

                return board;
            }

            throw new NotImplementedException();
        }

        public BoardBuilderDatabaseCreator WithDbContext(AppDbContext dbContext)
        {
            return new BoardBuilderDatabaseCreator(dbContext);
        }

        public class BoardBuilderDatabaseCreator
        {
            private IList<Board> _boards = new List<Board>();
            private IList<Player> _players = new List<Player>();
            private Game _configuredgame;
            private readonly AppDbContext _dbContext;

            public BoardBuilderDatabaseCreator(AppDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public BoardBuilderDatabaseCreator CreateBoard(int numberOfColumn = 3, int numberOfRows = 3)
            {
                _boards.Add(new Board {NumberOfColumn = numberOfColumn, NumberOfRows = numberOfRows});
                return this;
            }

            public BoardBuilderDatabaseCreator WithPlayers(params Player[] players)
            {
                foreach (var player in players)
                    _players.Add(player);

                return this;
            }

            public BoardBuilderDatabaseCreator CreateGame(bool completed = false, bool draw = false)
            {
                _configuredgame = new Game()
                {
                    Finished = completed,
                    Draw = draw
                };
                return this;
            }

            public async Task<IList<Board>> Build(bool clearOldData = true)
            {
                foreach (var board in _boards)
                {
                    board.PlayerBoards = new List<PlayerBoard>();
                    foreach (var player in _players)
                    {
                        _dbContext.Players.Add(player);
                        var playerBoard = new PlayerBoard {Player = player, Board = board};
                        var p = await _dbContext.Players.FindAsync(player.Id);
                        playerBoard.Player = p;
                        board.PlayerBoards.Add(playerBoard);
                    }

                    _dbContext.Boards.Add(board);
                }

                await _dbContext.SaveChangesAsync();

                return _boards;
            }
        }
    }
}
