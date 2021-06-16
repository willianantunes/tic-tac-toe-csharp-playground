#!/usr/bin/env bash

# https://www.willianantunes.com/blog/2021/05/production-ready-shell-startup-scripts-the-set-builtin/
set -eu -o pipefail

if [ -z "$1" ]; then
  echo "Please provide the sonar token 👀"
  exit 0
fi

if [ -z "$2" ]; then
  echo "Please provide the project version 👀"
  exit 0
fi

echo "### Reading variables..."
SONAR_TOKEN=$1
PROJECT_VERSION=$2

# You should start the scanner prior building your project and running your tests
dotnet sonarscanner begin \
    /k:"willianantunes_tic-tac-toe-csharp-playground" \
    /o:"willianantunes" \
    /d:sonar.login="$SONAR_TOKEN" \
    /v:"$PROJECT_VERSION" \
    /d:sonar.host.url="https://sonarcloud.io" \
    /d:sonar.cs.opencover.reportsPaths="**/*/coverage.opencover.xml" \
    /d:sonar.cs.vstest.reportsPaths="**/*/*.trx" \
    /d:sonar.coverage.exclusions="**/Migrations/*.cs" \
    /d:sonar.cpd.exclusions="**/Migrations/*.cs"

dotnet build
./scripts/start-tests.sh

# Now we can collect the results 👍
dotnet sonarscanner end /d:sonar.login="$SONAR_TOKEN"
