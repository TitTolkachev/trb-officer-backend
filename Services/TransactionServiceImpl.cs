using System.Globalization;
using Grpc.Core;

namespace trb_officer_backend.Services;

public class TransactionServiceImpl : TransactionService.TransactionServiceBase
{
    private readonly ILogger<TransactionServiceImpl> _logger;
    private readonly Helper _helper;

    public TransactionServiceImpl(ILogger<TransactionServiceImpl> logger, Helper helper)
    {
        Console.WriteLine("TransactionServiceImpl Init");
        _logger = logger;
        _helper = helper;
    }

    public override Task GetTransactionList(GetTransactionListRequest request,
        IServerStreamWriter<Transaction> responseStream,
        ServerCallContext context)
    {
        var key = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        _logger.LogInformation("GetTransactionList NEW CONSUMER");
        _helper.AddStream(key, responseStream);

        try
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        _helper.RemoveStream(key, responseStream);
        _logger.LogInformation("GetTransactionList REMOVED CONSUMER");

        return Task.CompletedTask;
    }
}