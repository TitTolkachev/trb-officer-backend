using Grpc.Core;
using trb_officer_backend.Common;

namespace trb_officer_backend.Services;

public class LoanServiceImpl : LoanService.LoanServiceBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<LoanServiceImpl> _logger;

    public LoanServiceImpl(IHttpClientFactory httpClientFactory, ILogger<LoanServiceImpl> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public override async Task<GetLoanListReply> GetLoanList(GetLoanListRequest request,
        ServerCallContext context)
    {
        // var httpClient = _httpClientFactory.CreateClient(Constants.LoanHttpClient);
        // var content = new Page (PageNumber : 0, PageSize : 10000);
        //
        // var response = await httpClient.GetAsync("loan", content);
        // if (!response.IsSuccessStatusCode)
        //     _logger.LogInformation("GetAccountList FAILED: {Response}", response.ToString());
        // response.EnsureSuccessStatusCode();
        //
        // var list = await response.Content.ReadFromJsonAsync<List<Dto.LoanShort>>();
        // if (list == null)
        //     throw new Exception("GetAccountList FAILED: list == null");
        //
        // return new GetLoanListReply {  Loan = list.ConvertAll(ConvertToProto) };
        return new GetLoanListReply();
    }

    public override async Task<GetLoanReply> GetLoan(GetLoanRequest request,
        ServerCallContext context)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.LoanHttpClient);

        var response = await httpClient.GetAsync($"loan/{request.Id}");
        if (!response.IsSuccessStatusCode)
            _logger.LogInformation("GetLoan FAILED: {Response}", response.ToString());
        response.EnsureSuccessStatusCode();

        var loan = await response.Content.ReadFromJsonAsync<Dto.Loan>();
        if (loan == null)
            throw new Exception("GetLoan FAILED: loan == null");

        return new GetLoanReply { Loan = ConvertToProto(loan) };
    }

    private static Loan ConvertToProto(Dto.Loan loan)
    {
        return new Loan
        {
            Id = loan.Id,
            IssuedDate = loan.IssuedDate,
            RepaymentDate = loan.RepaymentDate,
            IssuedAmount = loan.IssuedAmount,
            AmountDebt = loan.AmountDebt,
            AccruedPenny = loan.AccruedPenny,
            LoanTermInDays = loan.LoanTermInDays,
            ClientId = loan.ClientId,
            AccountId = loan.AccountId,
            State = loan.State,
            Tariff = new Tariff
            {
                Id = loan.Tariff.Id,
                AdditionDate = loan.Tariff.AdditionDate,
                Name = loan.Tariff.Name,
                Description = loan.Tariff.Description,
                InterestRate = loan.Tariff.InterestRate,
                OfficerId = loan.Tariff.OfficerId,
            }
        };
    }
}