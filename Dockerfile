FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /app
COPY . /app

RUN dotnet publish -c Release -o out TicTacToeCSharpPlayground

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

RUN groupadd --system app-user && adduser --system --ingroup app-user app-user

WORKDIR /app
COPY --from=build --chown=app-user:app-user /app/out .

EXPOSE 80

ENTRYPOINT ["dotnet", "TicTacToeCSharpPlayground.dll"]
CMD ["--urls", "http://+:80"]
