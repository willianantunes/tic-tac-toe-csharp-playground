using System.Collections.Generic;
using System.Linq;
using TicTacToeCSharpPlayground.Core.Business;

namespace Tests.Support
{
    public class CustomPositionDecider : IPositionDecider
    {
        public int ChooseTheBestAvailablePositionFor(IEnumerable<int> positions)
        {
            return positions.First();
        }
    }
}
