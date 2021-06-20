#!/usr/bin/env bash

# https://www.willianantunes.com/blog/2021/05/production-ready-shell-startup-scripts-the-set-builtin/
set -eu -o pipefail

CSPROJ_PATH=./src

if [ -z "$1" ]; then
  echo "Please provide the runtime details 👀"
  exit 0
fi

echo "### Reading variables..."
RUNTIME=$1

# https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish
# https://docs.microsoft.com/en-us/dotnet/core/rid-catalog#linux-rids
# https://docs.microsoft.com/en-us/dotnet/core/deploying/single-file#publish-a-single-file-app---sample-project-file
# PublishSingleFile: Packages the app into a platform-specific single-file executable
# PublishTrimmed: Trims unused libraries to reduce the deployment size of an app when publishing a self-contained executable
# --no-restore is used to make good use of container layers
dotnet publish \
    --runtime $RUNTIME \
    --configuration Release \
    --output out \
    -p:PublishSingleFile=true \
    -p:PublishTrimmed=true \
    $CSPROJ_PATH
