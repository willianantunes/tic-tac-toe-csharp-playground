using System;

namespace TicTacToeCSharpPlayground.Core.Exceptions
{
    public class TicTacToeRequiredDataExceptions : Exception
    {
        public TicTacToeRequiredDataExceptions(string message) : base(message)
        {
            // Base class of all required data exceptions 💀
        }
    }

    public class PlayerNotFoundException : TicTacToeRequiredDataExceptions
    {
        public PlayerNotFoundException(string message) : base(message) { }
    }

    public class BoardNotFoundToBePlayedException : TicTacToeRequiredDataExceptions
    {
        public BoardNotFoundToBePlayedException(string message) : base(message) { }
    }
}
