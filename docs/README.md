# Restore Documentation

Welcome to the Restore e-commerce application documentation.

## Table of Contents

### Getting Started

1. [Client Documentation](CLIENT.md) - React 18 + TypeScript frontend
   - Setup and installation
   - Development workflow
   - Project structure
   - Available scripts

2. [Server Documentation](SERVER.md) - .NET 8 WebAPI backend
   - Architecture overview
   - API endpoints
   - Database setup
   - Development guidelines

3. [Docker Compose Guide](DOCKER.md) - Containerized deployment
   - Running with Docker
   - Service configuration
   - Environment variables
   - Troubleshooting

### Development Tools

4. [MCP Servers](MCP.md) - Model Context Protocol integration
   - Available MCP servers
   - Configuration
   - Usage examples

### Testing

5. [Testing Guide](testing/README.md) - Testing documentation and credentials
   - Test credentials setup
   - E2E testing workflow
   - Stripe test cards
   - Known issues and workarounds

### Stripe Payment Integration

6. [Stripe Configuration](STRIPE.md) - Complete setup guide
   - Account setup
   - API key configuration
   - Webhook configuration
   - Testing with Stripe CLI

7. [User Secrets Management](UserSecrets.md) - Secure credential storage
   - Setting up user secrets
   - Managing API keys
   - Environment-specific configuration

8. [Stripe Test Summary](STRIPE_TEST_SUMMARY.md) - E2E testing results
   - Test execution details
   - Payment verification
   - Known issues and workarounds

## Quick Links

- **Main README**: [../README.md](../README.md)
- **GitHub Repository**: https://github.com/John-Cassidy/Restore
- **Udemy Course**: https://www.udemy.com/course/learn-to-build-an-e-commerce-store-with-dotnet-react-redux/

## Project Structure

```
Restore/
├── client/              # React frontend application
├── server/              # .NET backend services
│   └── Services/
│       └── Restore/     # Main API project
├── docs/                # Documentation (you are here)
│   └── testing/         # Testing guides and credentials
└── docker-compose.yml   # Docker orchestration
```

## Contributing

When adding new documentation:

1. Create markdown files in this `docs/` folder
2. Update this README.md with a link to your new document
3. Update the main [README.md](../README.md) if the documentation is important for new users
4. Follow the existing documentation structure and formatting

## Support

For issues or questions:

- Check the relevant documentation file above
- Review the [GitHub Issues](https://github.com/John-Cassidy/Restore/issues)
- Refer to the [Udemy course](https://www.udemy.com/course/learn-to-build-an-e-commerce-store-with-dotnet-react-redux/)
