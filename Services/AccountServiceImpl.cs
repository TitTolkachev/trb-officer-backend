using Grpc.Core;
using trb_officer_backend.Common;
using trb_officer_backend.Dto;

namespace trb_officer_backend.Services;

public class AccountServiceImpl : AccountService.AccountServiceBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AccountServiceImpl> _logger;

    public AccountServiceImpl(IHttpClientFactory httpClientFactory, ILogger<AccountServiceImpl> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public override async Task<GetAccountListReply> GetAccountList(GetAccountListRequest request,
        ServerCallContext context)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.CoreHttpClient);

        var response = await httpClient.GetAsync($"users/{request.UserId}/accounts");
        if (!response.IsSuccessStatusCode)
            _logger.LogInformation("GetAccountList FAILED: {Response}", response.ToString());
        response.EnsureSuccessStatusCode();

        var list = await response.Content.ReadFromJsonAsync<List<Dto.Account>>();
        if (list == null)
            throw new Exception("GetAccountList FAILED: list == null");

        var a = new List<Account>();
        foreach (var account in list)
        {
            a.Add(await ConvertToProto(account));
        }

        return new GetAccountListReply { Accounts = { a } };
    }

    public override async Task<GetAccountReply> GetAccount(GetAccountRequest request,
        ServerCallContext context)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.CoreHttpClient);

        var response = await httpClient.GetAsync($"accounts/{request.AccountId}");
        if (!response.IsSuccessStatusCode)
            _logger.LogInformation("GetAccount FAILED: {Response}", response.ToString());
        response.EnsureSuccessStatusCode();

        var account = await response.Content.ReadFromJsonAsync<Dto.Account>();
        if (account == null)
            throw new Exception("GetAccount FAILED: account == null");

        return new GetAccountReply { Account = await ConvertToProto(account) };
    }

    private async Task<List<ShortLoan>> GetClientLoans(string clientId)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.LoanHttpClient);

        var response = await httpClient.GetAsync($"client-loans?clientId={clientId}");
        if (!response.IsSuccessStatusCode)
            _logger.LogInformation("GetClientLoans FAILED: {Response}", response.ToString());
        response.EnsureSuccessStatusCode();

        var list = await response.Content.ReadFromJsonAsync<List<ShortLoan>>();
        if (list == null)
            throw new Exception("GetClientLoans FAILED: loan == null");

        return list;
    }

    private async Task<Account> ConvertToProto(Dto.Account account)
    {
        var reply = new Account
        {
            Type = account.Type,
            Balance = account.Balance,
            ClientFullName = account.ClientFullName,
            ExternalClientId = account.ExternalClientId,
            CreationDate = account.CreationDate,
            IsClosed = account.IsClosed
        };
        if (account.ClosingDate.HasValue)
            reply.ClosingDate = account.ClosingDate.Value;
        if (account.Type == "LOAN")
        {
            var x = account.Id;
            var loans = await GetClientLoans(account.ExternalClientId);
            foreach (var shortLoan in loans)
            {
                var httpClient = _httpClientFactory.CreateClient(Constants.LoanHttpClient);

                var response = await httpClient.GetAsync($"loan/{shortLoan.Id}");
                if (!response.IsSuccessStatusCode)
                    _logger.LogInformation("GetLoan FAILED: {Response}", response.ToString());
                response.EnsureSuccessStatusCode();

                var l = await response.Content.ReadFromJsonAsync<Dto.Loan>();
                if (l == null)
                    throw new Exception("GetLoan FAILED: l == null");

                if (l.AccountId == account.Id)
                    x = l.Id;
            }
            
            reply.Id = x;
        }
        else 
            reply.Id = account.Id;
        return reply;
    }
}