# Terminal Web Browser
> Work in progress

A minimal terminal-based web browser built from scratch in C# using raw TCP sockets. No HTTP libraries — just sockets, streams, and string parsing.

## How It Works
- Opens a raw TCP connection to the target host
- Wraps in `SslStream` for HTTPS support
- Manually builds and sends an HTTP/1.1 GET request
- Strips HTML tags and prints readable content to the console

## Usage
1. Clone the repo
2. Open in Visual Studio and run, or:
dotnet run
3. Enter a URL when prompted:
Enter Url: https://example.com

## Built With
- C# / .NET
- `TcpClient`, `SslStream` — no HTTP libraries
