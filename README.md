# Tic Tac Toe C# Playground

[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=willianantunes_tic-tac-toe-csharp-playground&metric=coverage)](https://sonarcloud.io/dashboard?id=willianantunes_tic-tac-toe-csharp-playground)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=willianantunes_tic-tac-toe-csharp-playground&metric=ncloc)](https://sonarcloud.io/dashboard?id=willianantunes_tic-tac-toe-csharp-playground)

This project is a full implementation of the Tic Tac Toe game but playable through a REST API. All the game data is saved in the database to make this challenge more fun. Run it now and play an honest game 🎮!

It's written in C# and uses ASP.NET Web API. Some more details:

- [CliFx](https://github.com/Tyrrrz/CliFx) is used to start the application. You can use it to start your worker, seed your database, and many more.
- [AutoMapper](https://github.com/AutoMapper/AutoMapper) is used by the Core layer to return DTO objects to the Api layer.
- [Serilog](https://github.com/serilog/serilog) handles everything concerning logging.
- [Npgsql](https://github.com/npgsql/npgsql/issues) is responsible to bridge our EF to PostgreSQL.

## Playing with the project

First run the project through the command:

    docker-compose up app-development

Then you can create players, a board and play a game! Below you can check some commands to play with the API.

```shell
# First create a player for you
http POST :8000/api/v1/players name=Jafar
# Then to someone who can play with you, like a computer
http POST :8000/api/v1/players name=Rose computer:=true
# You can list them
http GET :8000/api/v1/players
# Create your board
http POST :8000/api/v1/boards firstPlayerId=1 secondPlayerId=2
# Then you are good to go with a game!
# The URL has the following schema: /api/v1/games/{boardId}/{playerId}/{movementPosition}
http GET :8000/api/v1/games/1/1/1
```
