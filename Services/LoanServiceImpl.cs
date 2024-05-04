using Grpc.Core;
using trb_officer_backend.Common;
using trb_officer_backend.Dto;

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
        await FirebaseUtil.Validate(request.Token);

        var httpClient = _httpClientFactory.CreateClient(Constants.LoanHttpClient);

        var response = await httpClient.GetAsync("loan?pageNumber=0&pageSize=100");
        if (!response.IsSuccessStatusCode)
            _logger.LogInformation("GetLoanList FAILED: {Response}", response.ToString());
        response.EnsureSuccessStatusCode();

        var page = await response.Content.ReadFromJsonAsync<PageShortLoan>();
        if (page == null)
            throw new Exception("GetLoanList FAILED: page == null");

        return new GetLoanListReply { Loans = { ConvertToProto(page) } };
    }

    public override async Task<GetLoanReply> GetLoan(GetLoanRequest request,
        ServerCallContext context)
    {
        await FirebaseUtil.Validate(request.Token);

        var httpClient = _httpClientFactory.CreateClient(Constants.LoanHttpClient);

        var response = await httpClient.GetAsync($"loan/{request.LoanId}");
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
        var res = new Loan
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

        loan.Repayments.ForEach(r => res.Repayments.Add(new LoanRepayment
        {
            Id = r.Id,
            Date = r.Date,
            Amount = r.Amount,
            State = r.State,
        }));
        return res;
    }

    private static List<LoanShort> ConvertToProto(PageShortLoan page)
    {
        var res = new List<LoanShort>();
        page.Content.ForEach(loanShort => res.Add(new LoanShort
        {
            Id = loanShort.Id,
            IssuedDate = loanShort.IssuedDate,
            RepaymentDate = loanShort.RepaymentDate,
            AmountDebt = loanShort.AmountDebt,
            InterestRate = loanShort.InterestRate
        }));

        return res;
    }
}