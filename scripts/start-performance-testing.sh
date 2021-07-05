#!/usr/bin/env bash

# https://www.willianantunes.com/blog/2021/05/production-ready-shell-startup-scripts-the-set-builtin/
set -eu -o pipefail

LOGS_FOLDER_PATH=logs-jmeter
REPORTS_FOLDER_PATH=tests-jmeter

rm -rf $LOGS_FOLDER_PATH $REPORTS_FOLDER_PATH

# If you'd like to see all options type "jmeter -?" after you enter in the container: 
# docker run -it willianantunes/containerized-jmeter bash

jmeter --nongui \
    --testfile ./tests/PerformanceTesting/jmeter-test-plan.jmx \
    --jmeterproperty APP_ADDRESS=app \
    --jmeterproperty APP_PORT=8000 \
    --logfile $LOGS_FOLDER_PATH/log-sample-results.jtl \
    --jmeterlogfile $LOGS_FOLDER_PATH/log-run.jtl \
    --reportatendofloadtests \
    --reportoutputfolder $REPORTS_FOLDER_PATH
