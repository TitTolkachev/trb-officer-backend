using System.Net;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Http.Resilience;
using Polly;
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
    client => { client.BaseAddress = new Uri(Constants.CoreHost); })
    .AddResilienceHandler(
        "CustomPipeline",
        static builder =>
        {
            // See: https://www.pollydocs.org/strategies/retry.html
            builder.AddRetry(new HttpRetryStrategyOptions
            {
                // Customize and configure the retry logic.
                BackoffType = DelayBackoffType.Constant,
                MaxRetryAttempts = 10,
                UseJitter = true
            });

            // See: https://www.pollydocs.org/strategies/circuit-breaker.html
            builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
            {
                // Customize and configure the circuit breaker logic.
                SamplingDuration = TimeSpan.FromSeconds(30),
                FailureRatio = 0.7,
                MinimumThroughput = 10,
                ShouldHandle = static args => ValueTask.FromResult(args is
                {
                    Outcome.Result.StatusCode:
                    HttpStatusCode.RequestTimeout or
                    HttpStatusCode.TooManyRequests or
                    HttpStatusCode.InternalServerError
                })
            });

            // See: https://www.pollydocs.org/strategies/timeout.html
            builder.AddTimeout(TimeSpan.FromSeconds(10));
        });

builder.Services.AddHttpClient(Constants.LoanHttpClient,
    client => { client.BaseAddress = new Uri(Constants.LoanHost); })
    .AddResilienceHandler(
        "CustomPipeline",
        static builder =>
        {
            // See: https://www.pollydocs.org/strategies/retry.html
            builder.AddRetry(new HttpRetryStrategyOptions
            {
                // Customize and configure the retry logic.
                BackoffType = DelayBackoffType.Constant,
                MaxRetryAttempts = 10,
                UseJitter = true
            });

            // See: https://www.pollydocs.org/strategies/circuit-breaker.html
            builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
            {
                // Customize and configure the circuit breaker logic.
                SamplingDuration = TimeSpan.FromSeconds(30),
                FailureRatio = 0.7,
                MinimumThroughput = 10,
                ShouldHandle = static args => ValueTask.FromResult(args is
                {
                    Outcome.Result.StatusCode:
                    HttpStatusCode.RequestTimeout or
                    HttpStatusCode.TooManyRequests or
                    HttpStatusCode.InternalServerError
                })
            });

            // See: https://www.pollydocs.org/strategies/timeout.html
            builder.AddTimeout(TimeSpan.FromSeconds(10));
        });

builder.Services.AddHttpClient(Constants.UserHttpClient,
    client => { client.BaseAddress = new Uri(Constants.UserHost); })
    .AddResilienceHandler(
        "CustomPipeline",
        static builder =>
        {
            // See: https://www.pollydocs.org/strategies/retry.html
            builder.AddRetry(new HttpRetryStrategyOptions
            {
                // Customize and configure the retry logic.
                BackoffType = DelayBackoffType.Constant,
                MaxRetryAttempts = 10,
                UseJitter = true
            });

            // See: https://www.pollydocs.org/strategies/circuit-breaker.html
            builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
            {
                // Customize and configure the circuit breaker logic.
                SamplingDuration = TimeSpan.FromSeconds(30),
                FailureRatio = 0.7,
                MinimumThroughput = 10,
                ShouldHandle = static args => ValueTask.FromResult(args is
                {
                    Outcome.Result.StatusCode:
                    HttpStatusCode.RequestTimeout or
                    HttpStatusCode.TooManyRequests or
                    HttpStatusCode.InternalServerError
                })
            });

            // See: https://www.pollydocs.org/strategies/timeout.html
            builder.AddTimeout(TimeSpan.FromSeconds(10));
        });

builder.Services.AddHostedService<TransactionHandler>();

var app = builder.Build();

// Grpc Services
app.MapGrpcService<UserServiceImpl>();
app.MapGrpcService<TariffServiceImpl>();
app.MapGrpcService<ApplicationServiceImpl>();
app.MapGrpcService<LoanServiceImpl>();
app.MapGrpcService<AccountServiceImpl>();
app.MapGrpcService<TransactionServiceImpl>();
app.MapGrpcService<RatingServiceImpl>();

app.Run();