FROM confluentinc/cp-kafka-connect-base:7.3.2

RUN confluent-hub install --no-prompt confluentinc/kafka-connect-datagen:0.6.0 \
   && confluent-hub install --no-prompt confluentinc/kafka-connect-elasticsearch:14.0.5 \
   && confluent-hub install --no-prompt debezium/debezium-connector-sqlserver:2.0.1 \ 
   && confluent-hub install --no-prompt debezium/debezium-connector-mongodb:2.0.1 

COPY . /home/appuser/custom_schemas/

USER root
RUN chmod +x /home/appuser/custom_schemas/start-connectors.sh
USER appuser