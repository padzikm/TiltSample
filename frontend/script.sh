#!/bin/bash

if [[ -f ./lock ]]; then
    ec
else
    touch ./lock && nginx -g 
fi