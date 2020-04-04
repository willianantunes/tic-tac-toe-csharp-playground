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