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

            var positionsAsList = positions.ToList();
            var positionIndexThatIWillSuggest = random.Next(positionsAsList.Count());
            var chosenPosition = positionsAsList[positionIndexThatIWillSuggest];
            Log.Information("Will suggest the position {P}!", chosenPosition);

            return chosenPosition;
        }
    }
}
