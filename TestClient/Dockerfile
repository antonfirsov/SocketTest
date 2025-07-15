FROM mcr.microsoft.com/dotnet/nightly/sdk:10.0-preview as dotnet-sdk
FROM coredns/coredns:latest as coredns
FROM mcr.microsoft.com/dotnet-buildtools/prereqs:ubuntu-22.04-amd64

RUN apt update
RUN apt install -y wget net-tools nano sysvinit-utils dnsutils 

COPY --from=dotnet-sdk /usr/share/dotnet /usr/share/dotnet
ENV DOTNET_ROOT=/usr/share/dotnet
ENV PATH="${PATH}:/usr/share/dotnet"

COPY --from=coredns /coredns /usr/local/bin/coredns
RUN mkdir /etc/coredns
COPY coredns/Corefile /etc/coredns/Corefile
COPY coredns/address.test /etc/coredns/address.test

# Copy SysV init script for CoreDNS (compatible with service command)
COPY coredns/coredns /etc/init.d/coredns

# Make the init script executable and install it
RUN chmod +x /etc/init.d/coredns
RUN update-rc.d coredns defaults

WORKDIR /app
COPY . .

CMD ["/app/entrypoint.sh"]
