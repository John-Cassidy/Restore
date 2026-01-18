# Model Context Protocol (MCP) Servers

This document describes the MCP servers configured for this repository.

## Overview

MCP (Model Context Protocol) servers extend GitHub Copilot's capabilities by providing additional tools, prompts, and resources that can be accessed during chat sessions.

## Prerequisites

- **Docker Desktop** must be installed and running
- **VS Code** with GitHub Copilot extension
- **GitHub Copilot Chat** enabled

## Configured MCP Servers

### Awesome Copilot MCP Server

**Purpose**: AI-driven discovery and installation of GitHub Copilot customizations (instructions, agents, prompts, skills) from the [awesome-copilot](https://github.com/github/awesome-copilot) repository.

**Configuration File**: `.vscode/mcp.json`

**Container**: `ghcr.io/microsoft/mcp-dotnet-samples/awesome-copilot:latest`

#### Available Tools

- **`#search_instructions`**: Search GitHub Copilot customizations based on keywords
- **`#load_instruction`**: Load a specific customization from the repository

#### Available Prompts

- **`/mcp.awesome-copilot.get_search_prompt`**: Get a prompt for searching Copilot customizations

#### Usage Example

1. **Start the search workflow**:

   ```
   /mcp.awesome-copilot.get_search_prompt
   ```

2. **Enter search keywords** when prompted (e.g., "python", "docker", "testing")

3. **Review the results table** showing:

   - ✅ Already installed in your repository
   - ❌ Available to install
   - File type (instruction, agent, prompt, skill)
   - Filename and description

4. **Install a customization** by replying with the filename:

   ```
   python.instructions.md
   ```

5. **Copilot will**:
   - Load the content from the awesome-copilot repository
   - Save it to the appropriate directory (`.github/instructions/`, `.github/agents/`, etc.)
   - No modifications - saves exactly as published

#### Benefits

- **Conversational Discovery**: Ask Copilot to find customizations instead of manually browsing
- **Context-Aware**: Compares search results with your existing files
- **Automatic Installation**: No need to manually copy/paste files
- **Always Current**: Uses latest published customizations from the official repository

## Starting MCP Servers

MCP servers configured in `.vscode/mcp.json` start automatically when:

- You open VS Code
- Docker Desktop is running
- You interact with GitHub Copilot Chat

To manually start/restart:

1. Open Command Palette (`Ctrl+Shift+P` or `F1`)
2. Type: `MCP: List Servers`
3. Select `awesome-copilot`
4. Click `Start Server`

## Troubleshooting

### Server Not Starting

**Symptom**: Server doesn't appear or fails to start

**Solutions**:

1. Ensure Docker Desktop is running
2. Check Docker can pull images:
   ```powershell
   docker pull ghcr.io/microsoft/mcp-dotnet-samples/awesome-copilot:latest
   ```
3. Reload VS Code window (`Ctrl+Shift+P` → "Reload Window")

### Slow Startup

**Symptom**: First use takes a long time

**Cause**: Docker is downloading the container image

**Solution**: Pre-pull the image:

```powershell
docker pull ghcr.io/microsoft/mcp-dotnet-samples/awesome-copilot:latest
```

### Search Returns No Results

**Symptom**: `/mcp.awesome-copilot.get_search_prompt` shows no results

**Solutions**:

1. Check your search keywords - try broader terms
2. Verify server is running (check MCP status in VS Code)
3. Try alternative keywords (e.g., "python" vs "py")

## Alternative Installation Methods

### Local .NET Build

If you prefer not to use Docker:

1. Clone the repository:

   ```powershell
   git clone https://github.com/microsoft/mcp-dotnet-samples.git
   cd mcp-dotnet-samples/awesome-copilot
   ```

2. Update `.vscode/mcp.json`:
   ```json
   {
     "mcpServers": {
       "awesome-copilot": {
         "type": "stdio",
         "command": "dotnet",
         "args": [
           "run",
           "--project",
           "C:/path/to/mcp-dotnet-samples/awesome-copilot/src/McpSamples.AwesomeCopilot.HybridApp"
         ]
       }
     }
   }
   ```

**Benefits**: Faster startup, no Docker dependency
**Drawbacks**: Manual updates required

### HTTP Mode (Advanced)

For remote/shared access:

1. Start the server:

   ```powershell
   cd mcp-dotnet-samples/awesome-copilot
   dotnet run --project ./src/McpSamples.AwesomeCopilot.HybridApp -- --http
   ```

2. Update `.vscode/mcp.json`:
   ```json
   {
     "mcpServers": {
       "awesome-copilot": {
         "type": "sse",
         "url": "http://localhost:5250/sse"
       }
     }
   }
   ```

## Related Documentation

- [Awesome Copilot Repository](https://github.com/github/awesome-copilot)
- [MCP Server Documentation](https://github.com/microsoft/mcp-dotnet-samples/tree/main/awesome-copilot)
- [MCP Official Announcement](https://developer.microsoft.com/blog/announcing-awesome-copilot-mcp-server)
- [VS Code Copilot Customization Docs](https://code.visualstudio.com/docs/copilot/copilot-customization)

## Current Copilot Customizations

See the main [README.md](README.md) for:

- Installed instruction files in `.github/instructions/`
- Custom agents in `.github/agents/`
- Skills in `.github/skills/`
