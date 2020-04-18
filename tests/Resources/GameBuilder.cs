using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using src.Repository;

namespace tests.Resources
{
    public class GameBuilder
    {
        private IServiceProvider _serviceProvider;
        private Board _board;
        private Player _playerTwo;
        private Player _playerOne;
        private Game _game;

        public GameBuilder WithCreatedScopeFromServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            return this;
        }

        public GameBuilder WithBoard(Board createdBoard)
        {
            _board = createdBoard;
            return this;
        }

        public GameBuilder WithPlayers(Player playerOne, Player playerTwo)
        {
            _playerOne = playerOne;
            _playerTwo = playerTwo;

            return this;
        }

        public GameBuilder PlayerOneWinning()
        {
            _game = new Game()
            {
                Draw = false,
                Finished = true,
                Winner = _playerOne,
                ConfiguredBoard = _board
            };

            return this;
        }

        public async Task<Game> Build(bool clearOldData = true)
        {
            using var testPreparationScope = _serviceProvider.CreateScope();
            var context = testPreparationScope.ServiceProvider.GetRequiredService<CSharpPlaygroundContext>();
            if (clearOldData)
            {
                context.Games.RemoveRange(context.Games);
                context.Movements.RemoveRange(context.Movements);
                context.Boards.RemoveRange(context.Boards);
                context.Players.RemoveRange(context.Players);
                await context.SaveChangesAsync();
            }
            
            var entityEntry = await context.Games.AddAsync(_game);
            
            await context.SaveChangesAsync();
            
            return entityEntry.Entity;
        }
    }
}
