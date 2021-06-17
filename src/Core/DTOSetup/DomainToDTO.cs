using System.Linq;
using AutoMapper;
using TicTacToeCSharpPlayground.Core.Models;

namespace TicTacToeCSharpPlayground.Core.DTOSetup
{
    public class DomainToDTO : Profile
    {
        public DomainToDTO()
        {
            CreateMap<PlayerBoard, PlayerBoardDTO>();
            // https://docs.automapper.org/en/latest/Custom-value-resolvers.html#customizing-the-source-value-supplied-to-the-resolver
            CreateMap<Board, BoardDTO>()
                .ForMember(destinationMember => destinationMember.Players,
                    memberOptions => memberOptions.MapFrom(
                        source => source.PlayerBoards.Select(pb => pb.Player)));
            CreateMap<Player, PlayerDTO>();
            CreateMap<Game, GameDTO>();
        }
    }
}
