using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using TicTacToeCSharpPlayground.Core.Business;
using TicTacToeCSharpPlayground.Core.Models;
using TicTacToeCSharpPlayground.Core.Repository;
using TicTacToeCSharpPlayground.Infrastructure.Database;
using TicTacToeCSharpPlayground.Infrastructure.Database.Repositories;

namespace Tests.Support
{
    public class BoardBuilder
    {
        private int _boardSize;
        private Player _player;
        private List<Player> _players = new List<Player>();
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

        public BoardBuilder AddPlayers(params Player[] players)
        {
            _players.AddRange(players);
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
            var board = new Board
            {
                Movements = new List<Movement>(),
                NumberOfRows = _boardSize,
                NumberOfColumn = _boardSize,
                PlayerBoards = new List<PlayerBoard>()
            };
            board.InitializeBoardConfiguration();
            var fields = board.FieldsConfiguration;
            if (_players.Any())
            {
                foreach (var player in _players)
                {
                    board.PlayerBoards.Add(new PlayerBoard { Player = player });
                }
            }

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

            return board;
        }

        public BoardBuilderDatabaseCreator WithDbContext(AppDbContext dbContext)
        {
            return new BoardBuilderDatabaseCreator(dbContext);
        }

        public class BoardBuilderDatabaseCreator
        {
            private IList<Board> _boards = new List<Board>();
            private IList<Player> _players = new List<Player>();
            private readonly AppDbContext _dbContext;

            public BoardBuilderDatabaseCreator(AppDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public BoardBuilderDatabaseCreator CreateBoard(int numberOfColumn = 3, int numberOfRows = 3)
            {
                _boards.Add(new Board { NumberOfColumn = numberOfColumn, NumberOfRows = numberOfRows });
                return this;
            }

            public BoardBuilderDatabaseCreator WithPlayers(params Player[] players)
            {
                foreach (var player in players)
                    _players.Add(player);

                return this;
            }

            public async Task<IList<Board>> Build()
            {
                foreach (var board in _boards)
                {
                    board.PlayerBoards = new List<PlayerBoard>();
                    foreach (var player in _players)
                    {
                        _dbContext.Players.Add(player);
                        await _dbContext.SaveChangesAsync();
                        var playerBoard = new PlayerBoard { Player = player, Board = board };
                        var p = await _dbContext.Players.FindAsync(player.Id);
                        playerBoard.Player = p;
                        board.PlayerBoards.Add(playerBoard);
                    }

                    _dbContext.Boards.Add(board);
                    board.InitializeBoardConfiguration();
                }

                await _dbContext.SaveChangesAsync();

                return _boards;
            }

            public async Task<Board> BuildAndGetFirstBoard()
            {
                var boards = await Build();
                return boards.First();
            }
        }
    }
}
