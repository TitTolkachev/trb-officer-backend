using trb_officer_backend.Common;
using trb_officer_backend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddHttpClient(Constants.CoreHttpClient, client =>
{
    client.BaseAddress = new Uri(Constants.CoreHost);
});
builder.Services.AddHttpClient(Constants.LoanHttpClient, client =>
{
    client.BaseAddress = new Uri(Constants.LoanHost);
});
builder.Services.AddHttpClient(Constants.UserHttpClient, client =>
{
    client.BaseAddress = new Uri(Constants.UserHost);
});

var app = builder.Build();

app.MapGrpcService<UserServiceImpl>();
app.MapGrpcService<TariffServiceImpl>();
app.MapGrpcService<ApplicationServiceImpl>();
app.MapGrpcService<LoanServiceImpl>();

app.Run();