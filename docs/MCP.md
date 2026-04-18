# Model Context Protocol (MCP) Servers

This document describes the MCP servers configured for this repository.

## Overview

MCP (Model Context Protocol) servers extend GitHub Copilot's capabilities by providing additional tools, prompts, and resources that can be accessed during chat sessions.

## Prerequisites

- **Docker Desktop** must be installed and running
- **VS Code** with GitHub Copilot extension
- **GitHub Copilot Chat** enabled

## Related Documentation

- [Awesome Copilot Repository](https://github.com/github/awesome-copilot)
- [Awesome Copilot MCP Server](https://github.com/microsoft/mcp-dotnet-samples/tree/main/awesome-copilot)
- [Microsoft MCP .NET Samples](https://github.com/microsoft/mcp-dotnet-samples)
- [MCP Official Announcement](https://developer.microsoft.com/blog/announcing-awesome-copilot-mcp-server)
- [VS Code Copilot Customization Docs](https://code.visualstudio.com/docs/copilot/copilot-customization)

## Awesome Copilot MCP Server

**Purpose**: AI-driven discovery and installation of GitHub Copilot customizations (instructions, agents, prompts, skills) from the [awesome-copilot](https://github.com/github/awesome-copilot) repository.

### Install Awesome Copilot MCP Server

Install and Start Awesome Copilot container

1. Pull the image:
   ```powershell
    docker pull ghcr.io/microsoft/mcp-dotnet-samples/awesome-copilot:latest
   ```
2. Run the MCP server app in a container
   --http: The switch that indicates to run this MCP server as a streamable HTTP type. When this switch is added, the MCP server URL is http://localhost:8080

   ```powershell
   docker run -i --rm -p 8060:8080 ghcr.io/microsoft/mcp-dotnet-samples/awesome-copilot:latest --http
   ```

3. Update `.vscode/mcp.json`:

```json
{
  "servers": {
    "awesome-copilot": {
      "type": "http",
      "url": "http://0.0.0.0:8060/mcp"
    }
  }
}
```

### Use Awesome Copilot MCP Server

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

2. **Enter search keywords** when prompted (e.g., "C#", "python", "docker", "testing")

3. **Review the results table** showing:

   | Status | Filename                     | Description   |
   | ------ | ---------------------------- | ------------- |
   | ✅     | agent1.agent.md              | Description 1 |
   | ❌     | instruction1.instructions.md | Description 1 |
   | ✅     | prompt1.prompt.md            | Description 1 |
   | ❌     | skill1/SKILL.md              | Description 1 |
   - ✅ Already installed in your repository
   - ❌ Available to install

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
