FROM mcr.microsoft.com/dotnet/nightly/sdk:10.0-preview as dotnet-sdk
FROM coredns/coredns:latest as coredns
FROM mcr.microsoft.com/dotnet-buildtools/prereqs:ubuntu-22.04-amd64

RUN apt update
RUN apt install -y wget net-tools nano dnsutils

COPY --from=dotnet-sdk /usr/share/dotnet /usr/share/dotnet
ENV DOTNET_ROOT=/usr/share/dotnet
ENV PATH="${PATH}:/usr/share/dotnet"

COPY --from=coredns /coredns /usr/local/bin/coredns

WORKDIR /app
COPY . .
RUN dotnet build -c Release

RUN chmod +x /app/start.sh
CMD ["/app/start.sh"]

