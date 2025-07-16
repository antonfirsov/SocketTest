## What is this?

A vibe-coded container setup that runs `System.Net.Sockets.Socket.ConnectAsync` tests on Linux. It uses `tc` to introduce artificial delay so we can simulate slow connections and define tests for Happy Eyeballs scenarios where IPv4 and IPv4 connection attempts are racing with each other.

## How to run

- You have to be on Linux
- Change `TESTRUNTIME` in `.env` to point to your locally-built runtime.
- Run `docker compose up --build --abort-on-container-exit` from the repo root.

## How is it implemented?

- The docker setup defines a `testclient` container and various test server containers (`server-v4-fast`, `server-v4-slow`, `server-v6-fast`, `server-v6-slow`). `testclient` runs Xunit tests which attempt to connect to the sockets hosted in the server containers.
- Each server is configured with `tc qdisc add dev eth0 root netem delay ${DELAY}ms` to delay TCP packets.
- `testclient` hosts a CoreDNS server to resolve predefined hostnames.
- The IP addresses and the hostnames are defined in the `.env` file.
- The IP addresses and the hostnames are used in `TestClient/ConnectTests.cs` test cases to test various (Parallel) connection behaviors.

