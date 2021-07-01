ARG PROJECT_NAME=TicTacToeCSharpPlayground
ARG RUNTIME=linux-x64
ARG MAIN_PROJECT_SRC=./src

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env

ARG MAIN_PROJECT_SRC
ARG RUNTIME

WORKDIR /app

# Restores (downloads) all NuGet packages from the main project
COPY ${MAIN_PROJECT_SRC}/*.csproj ${MAIN_PROJECT_SRC}/
RUN dotnet restore --runtime ${RUNTIME} ${MAIN_PROJECT_SRC} && mv ${MAIN_PROJECT_SRC}/obj .

COPY ${MAIN_PROJECT_SRC} ./src
COPY TicTacToeCSharpPlayground.sln ./
COPY scripts/build-production.sh ./scripts/
RUN mv obj ${MAIN_PROJECT_SRC}/obj

RUN ./scripts/build-production.sh ${RUNTIME}

FROM mcr.microsoft.com/dotnet/runtime-deps:5.0 AS runtime

ARG PROJECT_NAME

WORKDIR /app

RUN useradd appuser && chown appuser /app

USER appuser

COPY --chown=appuser --from=build-env /app/out .
COPY --chown=appuser scripts/*.sh ./scripts/

CMD [ "./scripts/start-web.sh" ]
