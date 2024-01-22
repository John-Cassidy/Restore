using Restore.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureServices();

var app = builder.Build();

await app.ConfigurePipeline(builder.Configuration);

app.Run();

// create public partial class Program
public partial class Program { }
