FROM mcr.microsoft.com/dotnet/nightly/sdk:10.0-preview as dotnet-sdk
FROM ubuntu:24.04

RUN apt update
RUN apt install -y libicu-dev wget net-tools nano iproute2

COPY --from=dotnet-sdk /usr/share/dotnet /usr/share/dotnet
ENV DOTNET_ROOT=/usr/share/dotnet
ENV PATH="${PATH}:/usr/share/dotnet"

WORKDIR /app
COPY . .
RUN dotnet build -c Release

# Copy and make executable the startup script
COPY start.sh /app/start.sh
RUN chmod +x /app/start.sh

#CMD ["bash"]
CMD ["/app/start.sh"]