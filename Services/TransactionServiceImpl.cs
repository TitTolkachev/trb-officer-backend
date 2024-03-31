using Confluent.Kafka;
using Grpc.Core;
using trb_officer_backend.Common;

namespace trb_officer_backend.Services;

public class TransactionServiceImpl : TransactionService.TransactionServiceBase
{
    public override async Task GetTransactionList(GetTransactionListRequest request,
        IServerStreamWriter<GetTransactionListReply> responseStream,
        ServerCallContext context)
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
            while (!context.CancellationToken.IsCancellationRequested)
            {
                try
                {
                    var message = consumer.Consume(cts.Token).Message;

                    var result = new GetTransactionListReply
                        { Transactions = { new Transaction { Result = message.Value } } };
                    await responseStream.WriteAsync(result);

                    Console.WriteLine($"Consumed message '{message.Value}'");
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Error occured: {e.Error.Reason}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            consumer.Close();
        }
    }
}