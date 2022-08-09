#!/bin/bash

if [[ -f ./lock ]]; then
    dotnet TiltDemoApi.dll
else
    service ssh restart && touch ./lock && dotnet TiltDemoApi.dll
fi