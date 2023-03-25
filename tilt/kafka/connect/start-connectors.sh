#!/bin/bash

# curl -X POST -H "Content-Type: application/json" --data @datagen-custom.json http://localhost:8083/connectors | jq

curl -X POST -H "Content-Type: application/json" --data @elasticsearch-sink.json http://localhost:8083/connectors | jq

curl -X POST -H "Content-Type: application/json" --data @debezium-mssql.json http://localhost:8083/connectors | jq