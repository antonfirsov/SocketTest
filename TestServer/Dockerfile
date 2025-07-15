FROM mcr.microsoft.com/dotnet-buildtools/prereqs:ubuntu-22.04-amd64
WORKDIR /app
COPY . .
RUN apt update 
RUN apt install net-tools

CMD ["/bin/bash"]