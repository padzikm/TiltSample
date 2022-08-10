k3d cluster create k3s-default --servers 3 --registry-create demoregistry -p '8080:80@loadbalancer' -p '51437:31437@server:0' -p '5672:30672@server:0' -p '15672:31672@server:0' --k3s-arg '--disable=traefik@server:0' --k3s-arg '--disable=traefik@server:1' --k3s-arg '--disable=traefik@server:2'

kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.3.0/deploy/static/provider/cloud/deploy.yaml

kubectl wait --namespace ingress-nginx \
  --for=condition=ready pod \
  --selector=app.kubernetes.io/component=controller \
  --timeout=120s