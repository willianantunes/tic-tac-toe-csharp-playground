namespace TicTacToeCSharpPlayground.Api.Controllers.V1
{
    public record CreateBoardDto(int FirstPlayerId, int SecondPlayerId, string BoardSize = "3x3");

    public record PlayGameDto(int BoardId, int PlayerId, int MovementPosition);
}
