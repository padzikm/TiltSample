# what to install
docker, k3d, kubectl, terraform, tilt, dotnet 6, nodejs
# how to prepare
make sure these ports are free: 8080, 51437, 5672, 15672, 5601, 16686, 9090, 3000, 22000, 22001
# how to setup on macos and linux
./k8s.sh
# how to setup on windows
./k8s.ps1
# how to run
cd tilt && tilt up
press space to open ui view (best view is detailed - switch in top left corner in tilt ui)
# how to work with
changes in code will be automatically detected by tilt - changed service will be rebuilded and redeployed on file save
save changed source code and observe tilt ui tab in browser:
gray means rebuilding and redeploying
green means everything is running correctly
red means there is error
if tilt ui shows error navigate to errored task in tilt ui (in detailed view mode) to see error details
if you don't know what to do refresh errored task in tilt ui for rebuilding or redeploying
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
connect via ssh with user root and password root
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
