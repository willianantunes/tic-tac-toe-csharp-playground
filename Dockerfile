ARG PROJECT_NAME=TicTacToeCSharpPlayground
ARG RUNTIME=linux-x64
ARG DOTNET_VERSION=7.0
ARG MAIN_PROJECT_SRC=./src

FROM mcr.microsoft.com/dotnet/sdk:$DOTNET_VERSION AS build-env

ARG MAIN_PROJECT_SRC
ARG RUNTIME

WORKDIR /app

# Restores (downloads) all NuGet packages from the main project
COPY ${MAIN_PROJECT_SRC}/*.csproj ${MAIN_PROJECT_SRC}/
RUN dotnet restore --locked-mode --runtime ${RUNTIME} ${MAIN_PROJECT_SRC} && mv ${MAIN_PROJECT_SRC}/obj .

COPY ${MAIN_PROJECT_SRC} ./src
COPY TicTacToeCSharpPlayground.sln ./
COPY appsettings.json ./
COPY scripts/build-production.sh ./scripts/

RUN ./scripts/build-production.sh ${RUNTIME}

FROM mcr.microsoft.com/dotnet/runtime-deps:$DOTNET_VERSION AS runtime

RUN apt-get update && apt-get install -y curl

ARG PROJECT_NAME

WORKDIR /app

RUN useradd appuser && chown appuser /app

USER appuser

COPY --chown=appuser --from=build-env /app/out .
COPY --chown=appuser scripts/*.sh ./scripts/

HEALTHCHECK --interval=10s --timeout=3s \
    CMD curl --fail http://localhost:$ASPNETCORE_SOCKET_BIND_PORT/healthcheck/readiness || exit 1

CMD [ "./scripts/start-web.sh" ]
