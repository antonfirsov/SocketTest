FROM mcr.microsoft.com/dotnet/nightly/sdk:10.0-preview as dotnet-sdk
FROM ubuntu:24.04

RUN apt update
RUN apt install -y libicu-dev wget net-tools nano

COPY --from=dotnet-sdk /usr/share/dotnet /usr/share/dotnet
ENV DOTNET_ROOT=/usr/share/dotnet
ENV PATH="${PATH}:/usr/share/dotnet"

WORKDIR /app
COPY . .
RUN dotnet build -c Release

#CMD ["bash"]
CMD ["/usr/share/dotnet/dotnet", "run", "-c", "Release", "--no-build"]