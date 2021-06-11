using TicTacToeCSharpPlayground.Api.ExceptionHandling;

namespace TicTacToeCSharpPlayground.Api.Controllers.V1
{
    public class InvalidGameIsNotPlayableAnymoreException : HttpException
    {
        public override int StatusCode { get; set; } = 400;
        public override string DefaultDetail { get; set; } = "Game not available to be played anymore";
    }

    public class InvalidBoardNotFoundToBePlayedException : HttpException
    {
        public override int StatusCode { get; set; } = 400;
        public override string DefaultDetail { get; set; } = "Board not found";
    }

    public class InvalidPlayerNotFoundException : HttpException
    {
        public override int StatusCode { get; set; } = 400;
        public override string DefaultDetail { get; set; } = "Player not found";
    }

    public class InvalidBoardConfigurationException : HttpException
    {
        public override int StatusCode { get; set; } = 400;
        public override string DefaultDetail { get; set; } = "Board configuration not valid";
    }
}
