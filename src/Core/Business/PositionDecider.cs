using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace TicTacToeCSharpPlayground.Core.Business
{
    public interface IPositionDecider
    {
        int ChooseTheBestAvailablePositionFor(IEnumerable<int> positions);
    }

    public class PositionDecider : IPositionDecider
    {
        public int ChooseTheBestAvailablePositionFor(IEnumerable<int> positions)
        {
            Random random = new();

            var positionThatIWillSuggest = random.Next(1, positions.Count() + 1);
            Log.Information("Will suggest the position {P}!", positionThatIWillSuggest);

            return positionThatIWillSuggest;
        }
    }
}
