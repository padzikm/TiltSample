FROM mcr.microsoft.com/dotnet/aspnet:6.0

RUN apt-get update && apt-get install -y openssh-server unzip wget curl
RUN mkdir /var/run/sshd
RUN echo 'root:root' | chpasswd
RUN sed -i 's/#PermitRootLogin prohibit-password/PermitRootLogin yes/g' /etc/ssh/sshd_config

EXPOSE 22

ARG RIDER_VER
WORKDIR /root/.local/share/JetBrains/RiderRemoteDebugger/$RIDER_VER

RUN if [ -n "$RIDER_VER" ]; then wget -O rider_debug_tools.zip "https://download.jetbrains.com/resharper/dotUltimate.$RIDER_VER/JetBrains.Rider.RemoteDebuggerUploads.linux-x64.$RIDER_VER.zip" \
    && unzip -n rider_debug_tools.zip \
    && rm rider_debug_tools.zip; \
    else echo 'skipping rider debugging tools'; fi

COPY . /app/out
WORKDIR /app/out

RUN chmod +x ./script.sh
RUN chmod +x ./ready.sh
RUN chmod +x ./live.sh
RUN chmod +x ./db.sh
RUN chmod +x ./efbundle

ENTRYPOINT exec ./script.sh
