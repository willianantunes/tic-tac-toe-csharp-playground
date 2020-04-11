using System.Collections.Generic;
using src.Business;
using src.Repository;

namespace tests.Resources
{
    public class BoardBuilder
    {
        private int _boardSize;
        private Player _player;
        private int _rowToStartFilling;
        private int _fillUntilColumn;

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

        public BoardBuilder FilledAllColumnsUntilColumn(int fillUntilColumn)
        {
            _fillUntilColumn = fillUntilColumn;
            return this;
        }

        public Board build()
        {
            var board = new Board();
            board.Movements = new List<Movement>();
            board.NumberOfRows = _boardSize;
            board.NumberOfColumn = _boardSize;
            var boardDealer = new BoardDealer();
            boardDealer.InitializeBoardConfiguration(board);

            var fields = board.FieldsConfiguration;
            for (var column = 0; column <= _fillUntilColumn; column++)
                fields[_rowToStartFilling][column] = _player;

            return board;
        }
    }
}
