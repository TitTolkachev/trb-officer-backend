using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using trb_officer_backend.Common;
using trb_officer_backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Firebase
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.GetApplicationDefault(),
    ProjectId = "trb-officer-android",
});

// Transaction/Kafka Helper
builder.Services.AddSingleton<Helper>();

// Grpc
builder.Services.AddGrpc();

// Logger
builder.Services.AddSingleton(sp => sp.GetRequiredService<ILoggerFactory>().CreateLogger("DefaultLogger"));

// Http Clients
builder.Services.AddHttpClient(Constants.CoreHttpClient,
    client => { client.BaseAddress = new Uri(Constants.CoreHost); });
builder.Services.AddHttpClient(Constants.LoanHttpClient,
    client => { client.BaseAddress = new Uri(Constants.LoanHost); });
builder.Services.AddHttpClient(Constants.UserHttpClient,
    client => { client.BaseAddress = new Uri(Constants.UserHost); });

builder.Services.AddHostedService<TransactionHandler>();

var app = builder.Build();


// var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
// lifetime.ApplicationStarted.Register(async () =>
// {
//     // invoke the message bus after the host is running
//     var bus = app.Services.GetRequiredService<TransactionHandler>();
//     await bus.StartAsync(new GreetingCommand("Khalid"));
// });


// var host = Host.CreateDefaultBuilder(args)
//     .ConfigureServices(services => { services.AddHostedService<TransactionHandler>(); })
//     .Build();
// await host.RunAsync();

// Grpc Services
app.MapGrpcService<UserServiceImpl>();
app.MapGrpcService<TariffServiceImpl>();
app.MapGrpcService<ApplicationServiceImpl>();
app.MapGrpcService<LoanServiceImpl>();
app.MapGrpcService<AccountServiceImpl>();
app.MapGrpcService<TransactionServiceImpl>();

app.Run();