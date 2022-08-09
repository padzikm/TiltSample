FROM mcr.microsoft.com/dotnet/aspnet:6.0

RUN apt-get update && apt-get install -y openssh-server
RUN mkdir /var/run/sshd
RUN echo 'root:root' | chpasswd
RUN sed -i 's/#PermitRootLogin prohibit-password/PermitRootLogin yes/g' /etc/ssh/sshd_config

EXPOSE 22

COPY . /app/out
WORKDIR /app/out
RUN chmod +x ./script.sh

ENTRYPOINT exec ./script.sh
