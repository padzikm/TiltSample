#!/bin/bash

if [[ -f ./lock ]]; then
    dotnet TiltDemoApi2.dll
else
    service ssh restart && touch ./lock && dotnet TiltDemoApi2.dll
fi