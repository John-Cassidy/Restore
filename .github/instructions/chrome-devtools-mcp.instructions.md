# Chrome DevTools MCP Server - Configuration Guide

## Overview

The Chrome DevTools MCP enables GitHub Copilot to inspect and interact with Chrome browser instances. This guide covers the configuration needed to run it successfully.

## Prerequisites

- Google Chrome installed
- Node.js and npm/npx installed
- GitHub Copilot extension with MCP support

## Quick Setup

The Chrome DevTools MCP requires Chrome to be running with remote debugging enabled **before** the MCP server starts.

### Start Chrome with Remote Debugging

```powershell
# Windows
$chromePath = "C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"
Start-Process -FilePath $chromePath -ArgumentList "--remote-debugging-port=9222", "--user-data-dir=$env:TEMP\chrome-mcp-profile"
```

```bash
# macOS/Linux
google-chrome --remote-debugging-port=9222 --user-data-dir=/tmp/chrome-mcp-profile &
```

### Reload VS Code

After starting Chrome, reload VS Code to connect the MCP:

- Press `Ctrl+Shift+P` → "Developer: Reload Window"

## Configuration Options

### Basic Configuration (Connect to Running Chrome)

Location: `%APPDATA%\Code\User\mcp.json` (Windows) or `~/Library/Application Support/Code/User/mcp.json` (macOS)

```json
{
  "servers": {
    "io.github.ChromeDevTools/chrome-devtools-mcp": {
      "type": "stdio",
      "command": "npx",
      "args": [
        "chrome-devtools-mcp@0.12.1",
        "--browserUrl",
        "http://127.0.0.1:9222"
      ],
      "gallery": "https://api.mcp.github.com",
      "version": "0.12.1"
    }
  }
}
```

### Auto-Connect Mode (Chrome 145+)

```json
{
  "args": ["chrome-devtools-mcp@0.12.1", "--autoConnect"]
}
```

Requires remote debugging enabled in Chrome at `chrome://inspect/#remote-debugging`.

### Headless Mode

Run without browser UI:

```json
{
  "args": ["chrome-devtools-mcp@0.12.1", "--headless"]
}
```

### Custom Chrome Path

```json
{
  "args": [
    "chrome-devtools-mcp@0.12.1",
    "--executablePath",
    "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
  ]
}
```

### Different Chrome Channel

```json
{
  "args": [
    "chrome-devtools-mcp@0.12.1",
    "--channel",
    "beta" // Options: stable, beta, dev, canary
  ]
}
```

### Viewport Size

```json
{
  "args": ["chrome-devtools-mcp@0.12.1", "--viewport", "1920x1080"]
}
```

### Isolated Profile

```json
{
  "args": ["chrome-devtools-mcp@0.12.1", "--isolated"]
}
```

Creates a temporary profile that's deleted when Chrome closes.

### Feature Categories

Disable specific tool categories:

```json
{
  "args": [
    "chrome-devtools-mcp@0.12.1",
    "--no-category-emulation",
    "--no-category-performance",
    "--no-category-network"
  ]
}
```

## Auto-Start Chrome (Optional)

⚠️ **Use only in trusted workspaces** - This task runs automatically when opening the workspace.

### VS Code Task

Create `.vscode/tasks.json` to auto-start Chrome when opening the workspace:

```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "Start Chrome Debug for MCP",
      "type": "shell",
      "command": "Start-Process",
      "args": [
        "-FilePath",
        "'C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe'",
        "-ArgumentList",
        "'--remote-debugging-port=9222', '--user-data-dir=${env:TEMP}\\chrome-mcp-profile'"
      ],
      "presentation": {
        "reveal": "silent"
      },
      "runOptions": {
        "runOn": "folderOpen"
      }
    }
  ]
}
```

## Security Considerations

⚠️ **IMPORTANT SECURITY WARNING**

The Chrome DevTools MCP exposes browser content to GitHub Copilot and **grants full control** of the Chrome instance, including:

- Page content, DOM, and JavaScript execution
- Network requests, responses, headers, and payloads
- Cookies, local storage, session storage, and IndexedDB
- Ability to execute arbitrary JavaScript in any page
- Access to all open tabs and their contents

**Best Practices**:

- ✅ **Only use on trusted development machines** - Never on production systems or shared computers
- ✅ **Always use isolated Chrome profiles** (`--isolated` or `--userDataDir`) - Keeps debugging separate from personal browsing
- ✅ **Never browse sensitive sites** while MCP is connected - No banking, email, or personal accounts
- ✅ **Localhost only** - Port 9222 should ONLY be accessible from `127.0.0.1`, never `0.0.0.0` or network interfaces
- ✅ **Close debugging sessions** when not actively using the MCP
- ✅ **Trusted workspaces only** - The auto-start task runs when opening a workspace
- ⚠️ **Multi-user systems** - On shared machines, other local users could potentially connect to the debugging port

## Common Configuration Patterns

### Development Mode (Recommended)

```json
{
  "args": [
    "chrome-devtools-mcp@0.12.1",
    "--browserUrl",
    "http://127.0.0.1:9222",
    "--userDataDir",
    "${env:TEMP}\\chrome-mcp-profile"
  ]
}
```

### CI/CD Mode

```json
{
  "args": [
    "chrome-devtools-mcp@0.12.1",
    "--headless",
    "--isolated",
    "--viewport",
    "1920x1080"
  ]
}
```

### Multi-Channel Testing

```json
{
  "servers": {
    "chrome-stable": {
      "command": "npx",
      "args": ["chrome-devtools-mcp@0.12.1", "--channel", "stable"]
    },
    "chrome-canary": {
      "command": "npx",
      "args": ["chrome-devtools-mcp@0.12.1", "--channel", "canary"]
    }
  }
}
```

## Notes

- Chrome must be running with debugging enabled **before** the MCP starts
- Keep the debugging Chrome instance running while using the MCP
- Reload VS Code window after starting Chrome to establish connection
- Use separate Chrome profiles to isolate debugging from regular browsing
