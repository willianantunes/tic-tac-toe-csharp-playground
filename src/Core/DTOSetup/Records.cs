using System.Collections.Generic;

namespace TicTacToeCSharpPlayground.Core.DTOSetup
{
    public record PlayerDTO(long Id, string Name, bool Computer);

    public record PlayerBoardDTO(PlayerDTO Player);

    public record GameDTO(long Id, PlayerDTO Winner, bool Draw, bool Finished, BoardDTO ConfiguredBoard);

    public class BoardDTO
    {
        public long Id { get; set; }
        public IList<PlayerDTO> Players { get; set; }
        public int NumberOfColumn { get; set; }
        public int NumberOfRows { get; set; }
        public IList<IList<PlayerDTO?>>? FieldsConfiguration { get; set; }
        public IList<int>? FreeFields { get; set; }
    }
}
