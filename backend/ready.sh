#!/bin/bash

if [ -f readycheck ]
then
    exit 0
else
    curl -if http://0.0.0.0/healthz/ready
    if [ "$?" = 0 ]
    then
        touch readycheck
        exit 0
    fi
    exit 1
fi
