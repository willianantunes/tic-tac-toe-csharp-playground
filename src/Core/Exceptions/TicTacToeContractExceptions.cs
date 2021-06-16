using System;

namespace TicTacToeCSharpPlayground.Core.Exceptions
{
    public class TicTacToeContractExceptions : Exception
    {
        public TicTacToeContractExceptions(string message) : base(message)
        {
            // Base class of all contract exceptions 💀
        }
    }

    public class InvalidBoardConfigurationException : TicTacToeContractExceptions
    {
        public InvalidBoardConfigurationException(string message) : base(message) { }
    }

    public class YouAreNotAllowedToPlayWithARobotException : TicTacToeContractExceptions
    {
        public YouAreNotAllowedToPlayWithARobotException(string message) : base(message) { }
    }
    public class GameIsNotPlayableException : TicTacToeContractExceptions
    {
        public GameIsNotPlayableException(string message) : base(message) { }
    }

    public class PositionNotAvailableException : TicTacToeContractExceptions
    {
        public PositionNotAvailableException(string message) : base(message) { }
    }
}
