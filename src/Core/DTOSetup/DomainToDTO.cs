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
            CreateMap<Board, BoardDTO>()
                .ForMember(destinationMember => destinationMember.Players, 
                    memberOptions => memberOptions.MapFrom(
                        source => source.PlayerBoards.Select(pb => pb.Player)));
            CreateMap<Movement, MovementDTO>();
            CreateMap<Player, PlayerDTO>();
        }
    }
}
