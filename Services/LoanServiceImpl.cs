using Grpc.Core;

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

        return new GetLoanListReply
        {
            Loan = { new List<LoanShort>() }
        };
    }
}