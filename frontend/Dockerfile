FROM nginx:stable-alpine
#COPY /pipelines/nginx.conf /etc/nginx/conf.d/default.conf

COPY ./nginx.conf /etc/nginx/templates/default.conf.template
COPY ./frontend /usr/share/nginx/html

# ${1:+1} is hack for empty string https://github.com/docker/compose/issues/4636#issuecomment-610479786
# The nginx template (default.conf) expects these environment variables to exists, else the sub_filter will fail
# These should be overwritten in docker-compose.yml and/or the values.yaml file for kubernetes

# ENV ENV_WEB_URL ${1:+1}
# ENV ENV_AUTHCONFIG_CLIENTID ${1:+1}
# ENV ENV_AUTHCONFIG_POLICY ${1:+1}
# ENV ENV_AUTHCONFIG_REDIRECTURL ${1:+1}
# ENV ENV_AUTHCONFIG_B2CTENANTNAME ${1:+1}

EXPOSE 80
ENTRYPOINT ["nginx", "-g", "daemon off;"]