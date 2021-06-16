using TicTacToeCSharpPlayground.Core.Models;

namespace TicTacToeCSharpPlayground.Core.Business
{
    public record BoardState(bool HasAWinner, bool IsDraw);

    public record BoardSituation(bool HasAWinner, bool SadlyFinishedWithDraw, Player Winner, bool Concluded);
}
