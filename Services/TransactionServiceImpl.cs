using System.Globalization;
using Grpc.Core;
using trb_officer_backend.Common;
using trb_officer_backend.Dto;

namespace trb_officer_backend.Services;

public class TransactionServiceImpl : TransactionService.TransactionServiceBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TransactionServiceImpl> _logger;
    private readonly Helper _helper;

    public TransactionServiceImpl(ILogger<TransactionServiceImpl> logger, Helper helper,
        IHttpClientFactory httpClientFactory)
    {
        Console.WriteLine("TransactionServiceImpl Init");
        _logger = logger;
        _helper = helper;
        _httpClientFactory = httpClientFactory;
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

    public override async Task<GetTransactionsHistoryResponse> GetTransactionsHistory(
        GetTransactionsHistoryRequest request,
        ServerCallContext context)
    {
        await FirebaseUtil.Validate(request.Token);

        var httpClient = _httpClientFactory.CreateClient(Constants.CoreHttpClient);

        var response = await httpClient.GetAsync($"accounts/{request.AccountId}/history?page=0&size=100");
        if (!response.IsSuccessStatusCode)
            _logger.LogInformation("GetTransactionsHistory FAILED: {Response}", response.ToString());
        response.EnsureSuccessStatusCode();

        var page = await response.Content.ReadFromJsonAsync<PageTransactionHistory>();
        if (page == null)
            throw new Exception("GetTransactionsHistory FAILED: page == null");

        return new GetTransactionsHistoryResponse
        {
            Transactions =
            {
                page.Elements.ConvertAll(Convert)
            }
        };
    }

    private static TransactionParsed Convert(Dto.Transaction transaction)
    {
        var parsed = new TransactionParsed
        {
            Id = transaction.Id,
            ExternalId = transaction.ExternalId,
            Date = transaction.Date,
            Amount = transaction.Amount,
            Currency = transaction.Currency,
            Type = transaction.Type,
        };
        if (transaction.PayerAccountId != null)
            parsed.PayerAccountId = transaction.PayerAccountId;
        if (transaction.PayeeAccountId != null)
            parsed.PayeeAccountId = transaction.PayeeAccountId;

        return parsed;
    }
}