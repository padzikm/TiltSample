# what to install
docker, k3d, kubectl, terraform, tilt
# how to prepare
make sure these ports are free: 8080, 51437, 5672, 15672, 5601, 22000, 22001
# how to setup on linux
./k8s.sh
# how to setup on windows
./k8s.ps1
# how to run
cd tilt && tilt up
press space to open ui view (best view is detailed - switch in top left corner in tilt ui)
# what services are available
http://localhost:8080/front - frontend (folder frontend)
http://localhost:8080/back1 - microservice back1 (folder backend)
http://localhost:8080/back2 - microservice back2 (folder backend2)
http://localhost:5672 - rabbitmq api
http://localhost:15672 - rabbitmq ui
http://localhost:5601 - kibana
http://localhost:16686 - jaeger
http://localhost:9090 - prometheus
http://localhost:3000 - grafana
tcp://127.0.0.1:51437 - mssql
# credentials
mssql - user: sa password: Password1.
rabbitmq - user: guest password: guest
kibana - user: elastic password: elastic
# how to see logs
logs from every app are in tilt ui when switched to detailed view (or in kibana for microservices only)
# how to debug backend
connect via ssh with user root and password root:
ssh port 22000 - backend back1
ssh port 22001 - backend back2
# how to debug frontend
conntect to chrome devtools from your ide
# how to stop without removing created resources
ctrl+c (kill tilt process)
# how to remove created resources (without k8s cluster)
cd tilt && tilt down
# how to remove created k8s cluster
k3d cluster delete k3s-default
