using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using TicTacToeCSharpPlayground.Core.DTOSetup;
using TicTacToeCSharpPlayground.Core.Models;
using Xunit;

namespace Tests.TicTacToeCSharpPlayground.Core.DTOSetup
{
    public class DomainToDtoITests
    {
        private readonly IMapper _mapper;

        public DomainToDtoITests()
        {
            void Configure(IMapperConfigurationExpression cfg) => cfg.AddProfile(new DomainToDTO());
            var mapperConfiguration = new MapperConfiguration(Configure);
            _mapper = new Mapper(mapperConfiguration);
        }

        [Fact]
        public void ShouldMapBoardToItsDto()
        {
            // Arrange
            var playerOne = new Player { Id = 1, Name = "Jafar", Computer = false };
            var playerTwo = new Player { Id = 2, Name = "Rose", Computer = true };
            var playerBoardOne = new PlayerBoard { Player = playerOne };
            var playerBoardTwo = new PlayerBoard { Player = playerTwo };
            var movement = new Movement { Position = 1, WhoMade = playerTwo };
            var board = new Board
            {
                Id = 1,
                Movements = new List<Movement> { movement },
                PlayerBoards = new List<PlayerBoard> { playerBoardOne, playerBoardTwo },
                NumberOfColumn = 3,
                NumberOfRows = 3
            };
            board.InitializeBoardConfiguration();
            // Act
            var boardDto = _mapper.Map<Board, BoardDTO>(board);
            // Assert
            boardDto.Id.Should().Be(board.Id);
            boardDto.NumberOfColumn.Should().Be(board.NumberOfColumn);
            boardDto.NumberOfRows.Should().Be(board.NumberOfRows);
            boardDto.FieldsConfiguration.Should().HaveCount(board.NumberOfRows);
            foreach (var columns in boardDto.FieldsConfiguration)
                columns.Should().HaveCount(board.NumberOfColumn);
            boardDto.FreeFields.Should().Equal(board.FreeFields);
            boardDto.Players.Should().HaveCount(2);
            var playerOneDto = boardDto.Players.First(p => p.Name == playerOne.Name);
            playerOneDto.Computer.Should().Be(playerOne.Computer);
            playerOneDto.Id.Should().Be(playerOne.Id);
            var playerTwoDto = boardDto.Players.First(p => p.Name == playerTwo.Name);
            playerTwoDto.Computer.Should().Be(playerTwo.Computer);
            playerTwoDto.Id.Should().Be(playerTwo.Id);
        }
    }
}
