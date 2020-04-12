using System.Collections.Generic;
using src.Business;
using src.Helper;
using src.Repository;

namespace tests.Resources
{
    public class BoardBuilder
    {
        private int _boardSize;
        private Player _player;
        private int _rowToStartFilling;
        private int _fillUntilColumn;
        private int _startFromRow;
        private int _defaultColumn;
        private int _fillUntilRow;

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

        public BoardBuilder GivenColumnAndRow(int row, int col)
        {
            _startFromRow = row;
            _defaultColumn = col;
            return this;
        }

        public BoardBuilder FilledAllRowsUntilRows(int fillUntilRow)
        {
            _fillUntilRow = fillUntilRow;
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

            if (_fillUntilRow.IsNotNull())
            {
                var fields = board.FieldsConfiguration;
                for (var row = _startFromRow; row <= _fillUntilRow; row++)
                {
                    fields[row][_defaultColumn] = _player;
                }

                return board;
            }

            if (_fillUntilColumn.IsNotNull())
            {
                var fields = board.FieldsConfiguration;
                for (var column = 0; column <= _fillUntilColumn; column++)
                    fields[_rowToStartFilling][column] = _player;

                return board;
            }

            throw new System.NotImplementedException();
        }
    }
}
