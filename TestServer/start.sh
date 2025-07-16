#!/bin/bash

export DOTNET_CLI_TELEMETRY_OPTOUT=1

# Set up network delay using tc (traffic control)
DELAY_MS=${DELAY:-0}
if [ "$DELAY_MS" -gt 0 ]; then
    echo "Setting up network delay of ${DELAY_MS}ms on eth0"
    # Add delay to outgoing packets on the primary network interface
    tc qdisc add dev eth0 root netem delay ${DELAY_MS}ms
    echo "Network delay configured successfully"
else
    echo "No network delay configured (DELAY=${DELAY_MS})"
fi

# Start the .NET application
exec /usr/share/dotnet/dotnet run -c Release --no-build
