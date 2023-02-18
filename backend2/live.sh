#!/bin/bash

if [ -f livecheck ]
then
    exit 0
else
    curl -if http://0.0.0.0/healthz/live
    if [ "$?" = 0 ]
    then
        touch livecheck
        exit 0
    fi
    exit 1
fi
