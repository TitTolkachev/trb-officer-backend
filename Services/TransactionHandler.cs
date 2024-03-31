using System.Collections.Concurrent;
using Confluent.Kafka;
using Grpc.Core;
using trb_officer_backend.Common;

namespace trb_officer_backend.Services;

public class TransactionHandler: BackgroundService
{
    private readonly ConcurrentDictionary<string, IServerStreamWriter<Transaction>> _streams;
    private readonly ILogger<TransactionHandler> _logger;
    private readonly Helper _helper;

    public TransactionHandler(ILogger<TransactionHandler> logger, Helper helper)
    {
        Console.WriteLine("TransactionHandler Init");
        _logger = logger;
        _helper = helper;
        _streams = new ConcurrentDictionary<string, IServerStreamWriter<Transaction>>();
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var conf = new ConsumerConfig
        {
            GroupId = "trb-officer",
            BootstrapServers = Constants.KafkaHost,
            AutoOffsetReset = AutoOffsetReset.Latest
        };
        
        var consumer = new ConsumerBuilder<Ignore, string>(conf).Build();
        consumer.Subscribe("transaction.callback");
        
        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        try
        {
            Console.WriteLine("TransactionHandler Started");
            while (!stoppingToken.IsCancellationRequested)
            {
                var message = await Task.Run(()=>consumer.Consume());
                await _helper.HandleMessage(message.Message.Value);
                Console.WriteLine($"Consumed message '{message.Message.Value}'");
            }
        }
        catch (ConsumeException e)
        {
            Console.WriteLine($"Error occured: {e.Error.Reason}");
        }
    }
}