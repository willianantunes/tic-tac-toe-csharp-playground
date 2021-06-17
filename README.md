# Tic Tac Toe C# Playground

[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=willianantunes_tic-tac-toe-csharp-playground&metric=coverage)](https://sonarcloud.io/dashboard?id=willianantunes_tic-tac-toe-csharp-playground)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=willianantunes_tic-tac-toe-csharp-playground&metric=ncloc)](https://sonarcloud.io/dashboard?id=willianantunes_tic-tac-toe-csharp-playground)

![It has a tic tac toe board with some moves and two players](docs/ttt-csharp-playground.png)

This project is a full implementation of the Tic Tac Toe game but playable through a REST API. All the game data is saved in the database to make this challenge more fun. Run it now and play an honest game 🎮!

It's written in C# and uses ASP.NET Web API. Some more details:

- [CliFx](https://github.com/Tyrrrz/CliFx) is used to start the application. You can use it to start your worker, seed your database, and many more.
- [AutoMapper](https://github.com/AutoMapper/AutoMapper) is used by the Core layer to return DTO objects to the Api layer.
- [Serilog](https://github.com/serilog/serilog) handles everything concerning logging.
- [Npgsql](https://github.com/npgsql/npgsql/issues) is responsible to bridge our EF to PostgreSQL.

## Playing a game!

First run the project through the command below. It will generate any migrations that are missing, apply them, and run the project afterwards ([check out the script responsible for that](scripts/start-web-development.sh)):

    docker-compose up app-development

Then you can create players, a board, and play your game! Below you can check some [HTTPie](https://httpie.io/) commands to play with the API.

```shell
# First create a player for you
http POST :8000/api/v1/players name=Jafar
# Then to someone who can play with you, let's say a computer
http POST :8000/api/v1/players name=Rose computer:=true
# You can list them
http GET :8000/api/v1/players
# In order to have a game, you first need to create a board
http POST :8000/api/v1/boards firstPlayerId=1 secondPlayerId=2
# Then you are good to go!
# If you are lucky, you can win with three movements like the following:
http GET :8000/api/v1/games/play BoardId==1 PlayerId==1 MovementPosition==1
http GET :8000/api/v1/games/play BoardId==1 PlayerId==1 MovementPosition==2
http GET :8000/api/v1/games/play BoardId==1 PlayerId==1 MovementPosition==3
# Get the game details given its ID:
http GET :8000/api/v1/games/1
```

Sample GIF that shows a winning game with 4 movements (positions 1, 7, 8, and 9):

![An animation that shows someone playing the game through the API with HTTPie](docs/ttt-winning-game.gif)

## Architecture details

This project has 4 layers:

- API: ASP.NET Core stuff.
- Core: All business logic.
- Infrastructure: The database is fully configured in this layer.
- EntryCommands: It's like the `Startup.cs`.

![It shows four layers which represent how the program was organized](docs/ttt-architecture.png) 

It's important to mention that I didn't use the standard way to start the project, given from the webapi boilerplate project. `CliFx` plays an essential role as the command handler of the program. The idea was to mimic how Django works because it's easy to understand and test.

About the entities:

![It has 4 tables which describe how the database was modelled](docs/ttt-entities.png)

You can check how I configured these 5 tables using the EF Fluent API in [AppDbContext class](https://github.com/willianantunes/tic-tac-toe-csharp-playground/blob/6d900c128a0032a9d1c9be03481a3c8825153024/src/Infrastructure/Database/AppDbContext.cs#L26-L32).

## Running tests and how I use the real database to handle parallel testing

To run all tests, you can simply execute the command:

    docker-compose up tests

It will wait until the database is up and execute all the tests using the project's actual database (PostgreSQL).

To allow each integration test to be executed isolated without worries of race conditions or dirty data, I create one dedicated database per method test. You can check how I handled it by looking at the class [IntegrationTestsFixture](https://github.com/willianantunes/tic-tac-toe-csharp-playground/blob/08ce0cd9c2c75931a369f7af3a49d82478756cd9/tests/Support/IntegrationTestsFixture.cs).

## Robot logic 

If you look at the [PositionDecider class](https://github.com/willianantunes/tic-tac-toe-csharp-playground/blob/c78d68642bced98161bbbfaffb8f8d871ffbc506/src/Core/Business/PositionDecider.cs#L13), you'll notice that I simply choose a random available position available from the list. During the tests, I used an even simpler version ([CustomPositionDecider
 class](https://github.com/willianantunes/tic-tac-toe-csharp-playground/blob/157dc10375a19e0aa00bf209b27227b4fbdf560f/tests/Support/CustomPositionDecider.cs#L7)) that merely select the first open position. I could create a test that asserts a winning movement with three movements only (7, 8, and 9) on a 3x3 board with this approach (see it in [this test in GameServiceITests class](https://github.com/willianantunes/tic-tac-toe-csharp-playground/blob/ca91927d303706b65611ab9c5628945f70f9fdd8/tests/TicTacToeCSharpPlayground/Core/Services/GameServiceITests.cs#L203-L250)).
