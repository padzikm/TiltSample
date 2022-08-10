#!/bin/sh

if [[ -f ./lock ]]; then
    nginx -s reload
else
    touch ./lock && nginx -g "daemon off;"
fi