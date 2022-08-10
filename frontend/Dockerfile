FROM nginx:stable-alpine

COPY ./script.sh /app/
RUN chmod +x /app/script.sh

COPY ./nginx.conf /etc/nginx/templates/default.conf.template
COPY ./frontend /usr/share/nginx/html

RUN ln -sf /dev/stdout /var/log/nginx/access.log
RUN ln -sf /dev/stderr /var/log/nginx/error.log

EXPOSE 80

ENTRYPOINT ["nginx", "-g", "daemon off;"]
