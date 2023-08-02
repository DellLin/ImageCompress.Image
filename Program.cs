using ImageCompress.Image.DBContents;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
// builder.WebHost.ConfigureKestrel(options =>
// {
//     // Setup a HTTP/2 endpoint without TLS.
//     options.ListenLocalhost(5164, o => o.Protocols =
//         HttpProtocols.Http2);
// });

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddSingleton<PostgreSqlTcp>();
builder.Services.AddDbContext<PostgresContext>((serviceProvider, options) =>
{
    var postgreSqlTcp = serviceProvider.GetService<PostgreSqlTcp>()!;
    string connectionString = postgreSqlTcp.NewPostgreSqlTCPConnectionString().ConnectionString;
    options.UseNpgsql(connectionString);
});

var app = builder.Build();



// Configure the HTTP request pipeline.
app.MapGrpcService<ImageService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
