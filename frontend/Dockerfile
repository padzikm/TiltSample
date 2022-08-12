FROM nginx:stable-alpine

COPY ./script.sh /app/
RUN chmod +x /app/script.sh

COPY ./nginx.conf /etc/nginx/templates/default.conf.template
COPY ./frontend /usr/share/nginx/html

RUN ln -sf /dev/stdout /var/log/nginx/access.log
RUN ln -sf /dev/stderr /var/log/nginx/error.log

# ${1:+1} is hack for empty string https://github.com/docker/compose/issues/4636#issuecomment-610479786
# The nginx template (default.conf) expects these environment variables to exists, else the sub_filter will fail
# These should be overwritten in docker-compose.yml and/or the values.yaml file for kubernetes

ENV ENV_BACK1_URL ${1:+1}

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
