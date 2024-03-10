using Grpc.Core;
using trb_officer_backend.Common;
using trb_officer_backend.Dto;

namespace trb_officer_backend.Services;

public class ApplicationServiceImpl : ApplicationService.ApplicationServiceBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ApplicationServiceImpl> _logger;

    public ApplicationServiceImpl(IHttpClientFactory httpClientFactory, ILogger<ApplicationServiceImpl> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public override async Task<GetApplicationListReply> GetApplicationList(GetApplicationListRequest request,
        ServerCallContext context)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.LoanHttpClient);

        var list = await GetApplicationList("loanApplicationState=UNDER_CONSIDERATION", httpClient);
        list.AddRange(await GetApplicationList("loanApplicationState=APPROVED", httpClient));
        list.AddRange(await GetApplicationList("loanApplicationState=REJECTED", httpClient));
        list.AddRange(await GetApplicationList("loanApplicationState=FAILED", httpClient));
        list.Sort((x, y) => y.CreationDate.CompareTo(x.CreationDate));

        return new GetApplicationListReply
        {
            Application =
            {
                list.ConvertAll(application => new ApplicationShort
                {
                    Id = application.Id,
                    LoanTermInDays = application.LoanTermInDays,
                    IssuedAmount = application.IssuedAmount,
                    InterestRate = application.Tariff.InterestRate
                })
            }
        };
    }

    public override async Task<GetApplicationReply> GetApplication(GetApplicationRequest request,
        ServerCallContext context)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.LoanHttpClient);

        var response = await httpClient.GetAsync($"loan-applications/{request.Id}");
        if (!response.IsSuccessStatusCode)
            _logger.LogInformation("GetApplication FAILED: {Response}", response.ToString());
        response.EnsureSuccessStatusCode();

        var application = await response.Content.ReadFromJsonAsync<Dto.Application>();
        if (application == null)
            throw new Exception("GetApplication FAILED: application == null");

        return new GetApplicationReply { Application = ConvertToProto(application) };
    }

    public override async Task<ApproveApplicationReply> ApproveApplication(ApproveApplicationRequest request,
        ServerCallContext context)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.LoanHttpClient);
        var content = new ApplicationApprove(OfficerId: request.UserId, LoanApplicationId: request.ApplicationId) ;

        var response = await httpClient.PostAsJsonAsync($"loan-applications/approve", content);
        if (!response.IsSuccessStatusCode)
            _logger.LogInformation("ApproveApplication FAILED: {Response}", response.ToString());
        response.EnsureSuccessStatusCode();

        var application = await response.Content.ReadFromJsonAsync<Dto.Application>();
        if (application == null)
            throw new Exception("ApproveApplication FAILED: application == null");

        return new ApproveApplicationReply { Application = ConvertToProto(application) };
    }

    public override async Task<RejectApplicationReply> RejectApplication(RejectApplicationRequest request,
        ServerCallContext context)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.LoanHttpClient);
        var content = new ApplicationReject(OfficerId: request.UserId, LoanApplicationId: request.ApplicationId) ;

        var response = await httpClient.PostAsJsonAsync($"loan-applications/reject", content);
        if (!response.IsSuccessStatusCode)
            _logger.LogInformation("RejectApplication FAILED: {Response}", response.ToString());
        response.EnsureSuccessStatusCode();

        var application = await response.Content.ReadFromJsonAsync<Dto.Application>();
        if (application == null)
            throw new Exception("RejectApplication FAILED: application == null");

        return new RejectApplicationReply { Application = ConvertToProto(application) };
    }

    private async Task<List<Dto.Application>> GetApplicationList(string queryParams, HttpClient httpClient)
    {
        var response = await httpClient.GetAsync($"loan-applications?{queryParams}");
        if (!response.IsSuccessStatusCode)
            _logger.LogInformation("GetApplicationList FAILED: {Response}", response.ToString());
        response.EnsureSuccessStatusCode();

        var list = await response.Content.ReadFromJsonAsync<List<Dto.Application>>();
        if (list == null)
            throw new Exception("GetApplicationList FAILED: list == null");

        return list;
    }

    private static Application ConvertToProto(Dto.Application a)
    {
        var application = new Application
        {
            Id = a.Id,
            CreationDate = a.CreationDate,
            LoanTermInDays = a.LoanTermInDays,
            IssuedAmount = a.IssuedAmount,
            ClientId = a.ClientId,
            State = ConvertToProto(a.State),
            Tariff = new Tariff
            {
                Id = a.Tariff.Id,
                AdditionDate = a.Tariff.AdditionDate,
                Name = a.Tariff.Name,
                Description = a.Tariff.Description,
                InterestRate = a.Tariff.InterestRate,
                OfficerId = a.Tariff.OfficerId,
            }
        };
        if (a.UpdatedDateFinal.HasValue)
            application.UpdatedDateFinal = a.UpdatedDateFinal.Value;
        if (a.OfficerId != null)
            application.OfficerId = a.OfficerId;
        
        return application;
    }

    private static ApplicationState ConvertToProto(string s)
    {
        return s switch
        {
            "UNDER_CONSIDERATION" => ApplicationState.UnderConsideration,
            "APPROVED" => ApplicationState.Approved,
            "REJECTED" => ApplicationState.Rejected,
            _ => ApplicationState.Failed
        };
    }
}