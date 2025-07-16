#!/bin/bash

export DOTNET_CLI_TELEMETRY_OPTOUT=1

echo "Replacing .NET runtime with test runtime..."
rm -rf /usr/share/dotnet/shared/Microsoft.NETCore.App/*
ln -sf /testruntime /usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.0

echo "Microsoft.NETCore.App contents:"
ls -la /usr/share/dotnet/shared/Microsoft.NETCore.App/

# Use C# to generate the Corefile from environment variables and alter /etc/resolv.conf
/usr/share/dotnet/dotnet run -c Release --no-build

echo "Starting CoreDNS..."
/usr/local/bin/coredns -conf /etc/coredns/Corefile &
COREDNS_PID=$!
sleep 2

# Check if CoreDNS is running
if kill -0 $COREDNS_PID 2>/dev/null; then
    echo "CoreDNS started successfully with PID: $COREDNS_PID"
else
    echo "Failed to start CoreDNS"
    exit 1
fi

# This will run the test cases in ConnectTests.cs
/usr/share/dotnet/dotnet test -c Release --no-build
# bash
