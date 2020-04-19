# Tic Tac Toe C# Playground

A full implemented Tic Tac Toe through .NET Core 3.1 Web API.

## How to run the project

Download it and at the root folder simply execute:

    docker-compose up app

See the logs:

    docker logs -f tic-tac-toe-csharp-playground_app_1

## How to play the game

First create a player and keep its ID:

```shell
curl --location --request POST 'http://localhost:8000/tic-tac-toe/players' \
--header 'Content-Type: application/json' \
--data-raw '{
    "name": "Jafar"
}'
```

And now a computer to player with you:

```shell
curl --location --request POST 'http://localhost:8000/tic-tac-toe/players' \
--header 'Content-Type: application/json' \
--data-raw '{
    "name": "Rose",
    "computer": true
}'
```

Create a new board using the Jafar ID (it'll select the computer player automatically for you) and keep its generated ID with you for the next step:

```shell
curl --location --request POST 'http://localhost:8000/tic-tac-toe/boards' \
--header 'Content-Type: application/json' \
--data-raw '{
    "firstPlayerId": "c2fa2dde-2add-4cef-9c53-95738e540144"
}'
```

Now you can execute your movement and start a game! The pattern is `/tic-tac-toe/games/{boardId}/{playerId}/{movementPosition}`. You can do it issuing:

```shell
curl --location --request \
GET 'http://localhost:8000/tic-tac-toe/games/9f313da6-b920-4826-9bff-ebbfdfe5f3c3/c2fa2dde-2add-4cef-9c53-95738e540144/1'
```

You'll receive the following response:

```json
{
    "id": "37076628-af0b-4fa0-b07d-d1e3a38e3e37",
    "winner": null,
    "draw": false,
    "finished": false,
    "configuredBoard": {
        "id": "9f313da6-b920-4826-9bff-ebbfdfe5f3c3",
        "movements": [
            {
                "id": "d8f02b25-3c73-4197-bc72-b5c76b0b4743",
                "position": 1,
                "whoMade": {
                    "id": "c2fa2dde-2add-4cef-9c53-95738e540144",
                    "playerBoards": [
                        {
                            "id": "d7f9e201-b7cf-4440-9edb-40e5320d2e7e",
                            "playerId": "c2fa2dde-2add-4cef-9c53-95738e540144",
                            "boardId": "9f313da6-b920-4826-9bff-ebbfdfe5f3c3"
                        }
                    ],
                    "name": "Jafar",
                    "computer": false
                }
            },
            {
                "id": "5c80a17f-40c8-45dd-a9ef-82db0bc145ea",
                "position": 2,
                "whoMade": {
                    "id": "fdddb691-9636-49ca-b007-05a40d1b8a04",
                    "playerBoards": [
                        {
                            "id": "baaf4836-22f5-4fef-b4e6-d623b03053a8",
                            "playerId": "fdddb691-9636-49ca-b007-05a40d1b8a04",
                            "boardId": "9f313da6-b920-4826-9bff-ebbfdfe5f3c3"
                        }
                    ],
                    "name": "Rose",
                    "computer": true
                }
            }
        ],
        "playerBoards": [
            {
                "id": "d7f9e201-b7cf-4440-9edb-40e5320d2e7e",
                "playerId": "c2fa2dde-2add-4cef-9c53-95738e540144",
                "player": {
                    "id": "c2fa2dde-2add-4cef-9c53-95738e540144",
                    "playerBoards": [],
                    "name": "Jafar",
                    "computer": false
                },
                "boardId": "9f313da6-b920-4826-9bff-ebbfdfe5f3c3"
            },
            {
                "id": "baaf4836-22f5-4fef-b4e6-d623b03053a8",
                "playerId": "fdddb691-9636-49ca-b007-05a40d1b8a04",
                "player": {
                    "id": "fdddb691-9636-49ca-b007-05a40d1b8a04",
                    "playerBoards": [],
                    "name": "Rose",
                    "computer": true
                },
                "boardId": "9f313da6-b920-4826-9bff-ebbfdfe5f3c3"
            }
        ],
        "numberOfColumn": 3,
        "numberOfRows": 3,
        "fieldsConfiguration": [
            [
                {
                    "id": "c2fa2dde-2add-4cef-9c53-95738e540144",
                    "playerBoards": [
                        {
                            "id": "d7f9e201-b7cf-4440-9edb-40e5320d2e7e",
                            "playerId": "c2fa2dde-2add-4cef-9c53-95738e540144",
                            "boardId": "9f313da6-b920-4826-9bff-ebbfdfe5f3c3"
                        }
                    ],
                    "name": "Jafar",
                    "computer": false
                },
                {
                    "id": "fdddb691-9636-49ca-b007-05a40d1b8a04",
                    "playerBoards": [
                        {
                            "id": "baaf4836-22f5-4fef-b4e6-d623b03053a8",
                            "playerId": "fdddb691-9636-49ca-b007-05a40d1b8a04",
                            "boardId": "9f313da6-b920-4826-9bff-ebbfdfe5f3c3"
                        }
                    ],
                    "name": "Rose",
                    "computer": true
                },
                null
            ],
            [
                null,
                null,
                null
            ],
            [
                null,
                null,
                null
            ]
        ],
        "freeFields": [
            3,
            4,
            5,
            6,
            7,
            8,
            9
        ]
    }
}
```

## Development environment

I do not use an in-memory database to run integration tests, instead I prefer the real database to avoid surprises in production, then before develop run the following:

    docker-compose up -d db

## Running all tests

Execute the following command:

    docker-compose up tests

And you'll see something like:

```text
Test Run Successful.
Total tests: 44
     Passed: 44
 Total time: 10.8781 Seconds
```

## Useful links

Sample template projects:

- [jasontaylordev/NorthwindTraders](https://github.com/jasontaylordev/NorthwindTraders)
- [jasontaylordev/CleanArchitecture](https://github.com/jasontaylordev/CleanArchitecture)
- [nbarbettini/BeautifulRestApi](https://github.com/nbarbettini/BeautifulRestApi)
- [Boriszn/DeviceManager.Api](https://github.com/Boriszn/DeviceManager.Api)
- [auth0-samples/auth0-aspnetcore-webapi-samples](https://github.com/auth0-samples/auth0-aspnetcore-webapi-samples)
- [lkurzyniec/netcore-boilerplate](https://github.com/lkurzyniec/netcore-boilerplate)
- [fkhoda/checkout-shoppinglist-api](https://github.com/fkhoda/checkout-shoppinglist-api)
- [yanpitangui/netcore-api-boilerplate](https://github.com/yanpitangui/netcore-api-boilerplate)
- [kolappannathan/dotnet-core-web-api-boilerplate](https://github.com/kolappannathan/dotnet-core-web-api-boilerplate)

Interesting projects:

- [json-api-dotnet/JsonApiDotNetCore](https://github.com/json-api-dotnet/JsonApiDotNetCore)
- [cwetanow/DailyCodingProblem](https://github.com/cwetanow/DailyCodingProblem)
- [RyuzakiH/CloudflareSolverRe](https://github.com/RyuzakiH/CloudflareSolverRe)
- [Instantly generate C# models and helper methods from JSON](https://quicktype.io/csharp/)

Tutorials about some tools and best practices:

- [Efficient api calls with HttpClient and JSON.NET
](https://johnthiriet.com/efficient-api-calls/)
- [xUnit.net: Global setup + teardown?](https://stackoverflow.com/questions/12976319/xunit-net-global-setup-teardown)
- [Tutorial: Create a web API with ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-3.1&tabs=visual-studio)
- [Create web APIs with ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.1)
- [Configuration in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1)
- [Avoiding Startup service injection in ASP.NET Core 3](https://andrewlock.net/avoiding-startup-service-injection-in-asp-net-core-3/)
- [Configuration in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1)

Database setup:

- [Npgsql getting started](https://www.npgsql.org/efcore/)
- [ASP.NET Core, Entity Framework Core with PostgreSQL Code First](https://medium.com/faun/asp-net-core-entity-framework-core-with-postgresql-code-first-d99b909796d7)
- [Migrations](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)
- [Tutorial: Using the migrations feature - ASP.NET MVC with EF Core](https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/migrations?view=aspnetcore-3.1)
- [Automatically Upgrading on Application Startup (MigrateDatabaseToLatestVersion Initializer)](https://docs.microsoft.com/en-us/ef/ef6/modeling/code-first/migrations/#automatically-upgrading-on-application-startup-migratedatabasetolatestversion-initializer)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/), which enables [apply migrations at runtime](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli#apply-migrations-at-runtime)

Logging:

- [Logging in .NET Core and ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1)

Testing:

- [Unit test controller logic in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/testing?view=aspnetcore-3.1)
- [Integration tests in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.1)

How to handle errors:

- [Handling errors in an ASP.NET Core Web API](https://www.devtrends.co.uk/blog/handling-errors-in-asp.net-core-web-api)

Articles which I'll use to refactor this project after its initial release:

- [Tackle Business Complexity in a Microservice with DDD and CQRS Patterns](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/)
  - [zkavtaskin/Domain-Driven-Design-Example](https://github.com/zkavtaskin/Domain-Driven-Design-Example)
 
About Rider IDE:

- [NuGet Package Manager Console Support](https://rider-support.jetbrains.com/hc/en-us/community/posts/360001346579/comments/360000194879)
- [Running Entity Framework (Core) commands in Rider](https://blog.jetbrains.com/dotnet/2017/08/09/running-entity-framework-core-commands-rider/)

## Some lessons

### EF

In order to use Entity Framework commands, I had to install through the following:
 
    dotnet tool install --global dotnet-ef

Then I could issue the command:

    dotnet ef migrations add InitialCreate
    
To scaffold a controller, first install these guys in `TicTacToeCSharpPlayground`:

    dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
    dotnet add package Microsoft.EntityFrameworkCore.SqlServer
    dotnet tool install --global dotnet-aspnet-codegenerator

The package `Microsoft.EntityFrameworkCore.SqlServer` can be uninstalled afterwards as we're using Postgres. Then you can do:

    dotnet aspnet-codegenerator controller -name TodoItemsController \
    --useAsyncActions --restWithNoViews \
    --model TodoItem --dataContext CSharpPlaygroundContext -outDir Controllers

### Build

At the root folder of the project, in order to generate a DLL of the project:

    dotnet publish -c Release -o out TicTacToeCSharpPlayground

Then, with the help of Docker, you can run your generated DLL:

    docker run -it --rm --name tic-tac-toe-csharp-playground \
    -p 8000:80 \
    -v $(pwd)/out:/app \
    -w /app \
    mcr.microsoft.com/dotnet/core/aspnet:3.1 \
    dotnet TicTacToeCSharpPlayground.dll 

With this project, you'll get an error because it needs a database.
