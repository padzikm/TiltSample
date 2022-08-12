FROM mcr.microsoft.com/dotnet/aspnet:6.0

RUN apt-get update && apt-get install -y openssh-server unzip wget
RUN mkdir /var/run/sshd
RUN echo 'root:root' | chpasswd
RUN sed -i 's/#PermitRootLogin prohibit-password/PermitRootLogin yes/g' /etc/ssh/sshd_config

EXPOSE 22

WORKDIR /root/.local/share/JetBrains/RiderRemoteDebugger/2022.1.2

RUN wget -O rider_debug_tools.zip https://data.services.jetbrains.com/products/download\?code\=RRD\&platform\=linux64
RUN unzip -n rider_debug_tools.zip

COPY . /app/out
WORKDIR /app/out
RUN chmod +x ./script.sh

ENTRYPOINT exec ./script.sh
