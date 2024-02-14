# Restore

e-commerce store with .Net, React &amp; Redux

[Udemy Course](https://www.udemy.com/course/learn-to-build-an-e-commerce-store-with-dotnet-react-redux/)
[Course GitHub Repo](https://github.com/TryCatchLearn/Restore)

## GitHub Action Badges

[![build and test](https://github.com/John-Cassidy/Restore/actions/workflows/build-and-test.yaml/badge.svg)](https://github.com/John-Cassidy/Restore/actions/workflows/build-and-test.yaml)

## Dev Container(s)

Configured devcontainer.json to create container to run both:

- .NET 8 WebApi solution - port 5000
- React 18 client application - port 3000
- PostgreSQL DB - [standard postgresql port]

[Devcontainer Setup with PostgreSQL DB](https://github.com/devcontainers/templates/tree/main/src/dotnet-postgres)

[PostgreSQL Image](https://hub.docker.com/_/postgres)

[Create Devcontainer](https://code.visualstudio.com/docs/devcontainers/create-dev-container)

[Developing Inside Container](https://code.visualstudio.com/docs/devcontainers/containers)

[Extension: Visual Studio Code Dev Containers](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers)

[Getting Started with .NET 8 DevContainer](https://betterprogramming.pub/getting-started-with-net-8-seamless-setup-with-devcontainers-13851ee20f4e)

[Example Setup](https://dev.to/this-is-learning/set-up-github-codespaces-for-a-net-8-application-5999)

### Securely Manage Secrets in Devcontainer using Environment Variables

To securely manage secrets in your devcontainer using environment variables, you can use a .env file. This file should not be checked into source control, and it should be used to store your secrets. Here's how you can do it:

1. Create a .env file in your local workspace (the same directory as your devcontainer.json and docker-compose.yml files). This file should contain your secrets in the form of environment variables:

```powershell
SECRET_KEY=your_secret_key
ANOTHER_SECRET=another_secret
```

In your docker-compose.yml file, you can use the env_file directive to load the environment variables from the .env file:

```yml
services:
  app:
    env_file:
      - .env
```

In your devcontainer.json file, you can reference these environment variables in the remoteEnv section:

```json
{
  "remoteEnv": {
    "ASPNETCORE_ENVIRONMENT": "Development",
    "DOTNET_USE_POLLING_FILE_WATCHER": "true",
    "TARGET": "net8.0",
    "SECRET_KEY": "${localEnv:SECRET_KEY}",
    "ANOTHER_SECRET": "${localEnv:ANOTHER_SECRET}"
  }
}
```

This way, your secrets are loaded into the environment of the devcontainer, but they are not checked into source control. Make sure to add .env to your .gitignore file to prevent it from being accidentally committed.
