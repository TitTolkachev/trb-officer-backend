using trb_officer_backend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddHttpClient("Main", client =>
{
    client.BaseAddress = new Uri("http://188.235.125.159:8082/api/v1/");
});

var app = builder.Build();

app.MapGrpcService<UserServiceImpl>();

app.Run();