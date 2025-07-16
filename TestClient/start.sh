#!/bin/bash
echo "Starting CoreDNS..."

# Check if CoreDNS binary exists
if [ ! -f /usr/local/bin/coredns ]; then
    echo "ERROR: CoreDNS binary not found at /usr/local/bin/coredns"
    exit 1
fi

# Check if CoreDNS config exists
if [ ! -f /etc/coredns/Corefile ]; then
    echo "ERROR: CoreDNS config not found at /etc/coredns/Corefile"
    exit 1
fi

# Start CoreDNS directly in background
echo "Starting CoreDNS directly..."
/usr/local/bin/coredns -conf /etc/coredns/Corefile &
COREDNS_PID=$!
sleep 2

# Check if CoreDNS is running
if kill -0 $COREDNS_PID 2>/dev/null; then
    echo "CoreDNS started successfully with PID: $COREDNS_PID"
    
    # Configure the container to use CoreDNS as primary nameserver
    echo "Configuring /etc/resolv.conf to use CoreDNS..."
    # Backup original resolv.conf
    cp /etc/resolv.conf /etc/resolv.conf.backup
    
    # Create new resolv.conf with CoreDNS as primary, original as fallback
    {
        echo "nameserver 127.0.0.1"
        # Add original nameservers as fallback
        grep "^nameserver" /etc/resolv.conf.backup | head -2
    } > /etc/resolv.conf
    
    echo "Updated /etc/resolv.conf:"
    cat /etc/resolv.conf
else
    echo "Failed to start CoreDNS"
fi

/usr/share/dotnet/dotnet run -c Release --no-build
# bash
