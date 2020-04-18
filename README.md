# Tic Tac Toe C# Playground

A full implemented Tic Tac Toe through .NET Core 3.1 Web API.

## How to run the project

Download it and at the root folder simply execute:

    docker-compose up app

See the logs:

    docker logs -f csharp-playground_app_1
    
## Development environment

I do not use an in-memory database to run integration tests, instead I prefer the real database to avoid surprises in production, then before develop run the following:

    docker-compose up -d db

## How to play the game

WIP.

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
