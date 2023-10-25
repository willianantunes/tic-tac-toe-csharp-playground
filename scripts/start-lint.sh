#!/usr/bin/env bash

# https://www.willianantunes.com/blog/2021/05/production-ready-shell-startup-scripts-the-set-builtin/
set -eu -o pipefail

dotnet format --verify-no-changes --verbosity d
