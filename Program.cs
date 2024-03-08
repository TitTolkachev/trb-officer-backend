using trb_officer_backend.Common;
using trb_officer_backend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddHttpClient(Constants.UserHttpClient, client =>
{
    client.BaseAddress = new Uri(Constants.UserHost);
});
builder.Services.AddHttpClient(Constants.CoreHttpClient, client =>
{
    client.BaseAddress = new Uri(Constants.CoreHost);
});

var app = builder.Build();

app.MapGrpcService<UserServiceImpl>();

app.Run();