# C# Playground

I'm working on it, so this is WIP.

## What you'll see here

A Tic Tac Toe game playable through REST endpoints. Some contracts:

- `GET /players`: return all players configured
- `POST /players`: create a new player
- `GET /boards`: return all boards configured.
- `POST /boards`: create a new board. You should provide your name and a custom board configuration which defaults to 3x3.
- `GET /games`: return all games configured informing when each one started and finished if applicable.  
- `GET /games/{boardId}`: return the status of the given board.
- `POST /games/{boardId}`: If the game hasn't finished, you can apply a position. The response will inform the result given the computer move too.

Yeah, this may lack some requirements, but it's just an idea to explore C# language. I thank you [IGU](https://github.com/igooorgp) about his idea.

## How to work with the project

First you must do the following:

    docker-compose up -d db
    
Then you can execute the project in your IDE.

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

Database setup:

- [Npgsql getting started](https://www.npgsql.org/efcore/)
- [ASP.NET Core, Entity Framework Core with PostgreSQL Code First](https://medium.com/faun/asp-net-core-entity-framework-core-with-postgresql-code-first-d99b909796d7)
- [Migrations](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)
- [Tutorial: Using the migrations feature - ASP.NET MVC with EF Core](https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/migrations?view=aspnetcore-3.1)
- [Automatically Upgrading on Application Startup (MigrateDatabaseToLatestVersion Initializer)](https://docs.microsoft.com/en-us/ef/ef6/modeling/code-first/migrations/#automatically-upgrading-on-application-startup-migratedatabasetolatestversion-initializer)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/), which enables [apply migrations at runtime](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli#apply-migrations-at-runtime)

Articles which I'll use to refactor this project after its initial release:

- [Tackle Business Complexity in a Microservice with DDD and CQRS Patterns](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/)
  - [zkavtaskin/Domain-Driven-Design-Example](https://github.com/zkavtaskin/Domain-Driven-Design-Example)
 
About Rider IDE:

- [NuGet Package Manager Console Support](https://rider-support.jetbrains.com/hc/en-us/community/posts/360001346579/comments/360000194879)
- [Running Entity Framework (Core) commands in Rider](https://blog.jetbrains.com/dotnet/2017/08/09/running-entity-framework-core-commands-rider/)

## Some lessons
 
In order to use Entity Framework commands, I had to install through the following:
 
    dotnet tool install --global dotnet-ef

Then I could issue the commands:

    dotnet ef migrations add InitialCreate