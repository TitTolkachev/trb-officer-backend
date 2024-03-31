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

// Kafka
// builder.Services.AddKafka(kafka => kafka
//     .AddCluster(cluster =>
//     {
//         const string hostName = Constants.KafkaHostName;
//         const string topicName = "transaction.callback";
//         cluster
//             .WithBrokers(new[] { hostName })
//             .CreateTopicIfNotExists(topicName, 1, 1)
//             .AddConsumer(consumer =>
//                 consumer
//                     .Topic(topicName)
//                     .WithGroupId("notifications")
//                     .AddMiddlewares(middlewares => middlewares
//                         .AddDeserializer<JsonCoreDeserializer>()
//                         .AddTypedHandlers(handlers =>
//                             handlers.AddHandler<AddTaskHandler>()
//                         )
//                     )
//             );
//     })
// );

// Grpc
builder.Services.AddGrpc();

// Http Clients
builder.Services.AddHttpClient(Constants.CoreHttpClient,
    client => { client.BaseAddress = new Uri(Constants.CoreHost); });
builder.Services.AddHttpClient(Constants.LoanHttpClient,
    client => { client.BaseAddress = new Uri(Constants.LoanHost); });
builder.Services.AddHttpClient(Constants.UserHttpClient,
    client => { client.BaseAddress = new Uri(Constants.UserHost); });

var app = builder.Build();

// Kafka Bus
// var kafkaBus = app.Services.CreateKafkaBus();
// await kafkaBus.StartAsync();

// Grpc Services
app.MapGrpcService<UserServiceImpl>();
app.MapGrpcService<TariffServiceImpl>();
app.MapGrpcService<ApplicationServiceImpl>();
app.MapGrpcService<LoanServiceImpl>();
app.MapGrpcService<AccountServiceImpl>();
app.MapGrpcService<TransactionServiceImpl>();

app.Run();