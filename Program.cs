using trb_officer_backend.Common;
using trb_officer_backend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddHttpClient(Constants.CoreHttpClient, client =>
{
    client.BaseAddress = new Uri(Constants.CoreHost);
});
builder.Services.AddHttpClient(Constants.TariffHttpClient, client =>
{
    client.BaseAddress = new Uri(Constants.TariffHost);
});
builder.Services.AddHttpClient(Constants.UserHttpClient, client =>
{
    client.BaseAddress = new Uri(Constants.UserHost);
});

var app = builder.Build();

app.MapGrpcService<UserServiceImpl>();

app.Run();