curl -X POST -H "Content-Type: application/json" --data @connect/datagen-users.json http://localhost:8083/connectors

curl -X POST -H "Content-Type: application/json" --data @connect/elasticsearch-sink.json http://localhost:8083/connectors

curl -X POST -H "Content-Type: application/json" --data @connect/debezium-mssql.json http://localhost:8083/connectors