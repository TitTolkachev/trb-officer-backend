using Grpc.Core;
using trb_officer_backend.Common;
using trb_officer_backend.Dto;

namespace trb_officer_backend.Services;

public class TariffServiceImpl : TariffService.TariffServiceBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TariffServiceImpl> _logger;

    public TariffServiceImpl(IHttpClientFactory httpClientFactory, ILogger<TariffServiceImpl> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public override async Task<GetTariffListReply> GetTariffList(GetTariffListRequest request,
        ServerCallContext context)
    {
        await FirebaseUtil.Validate(request.Token);

        var httpClient = _httpClientFactory.CreateClient(Constants.LoanHttpClient);

        var response = await httpClient.GetAsync("tariff");
        if (!response.IsSuccessStatusCode)
            _logger.LogInformation("GetClientList FAILED: {Response}", response.ToString());
        response.EnsureSuccessStatusCode();

        var list = await response.Content.ReadFromJsonAsync<List<Tariff>>();
        if (list == null)
            throw new Exception("GetTariffList FAILED: list == null");

        return new GetTariffListReply
        {
            Tariffs =
            {
                list.ConvertAll(tariff => new Tariff
                {
                    Id = tariff.Id,
                    AdditionDate = tariff.AdditionDate,
                    Name = tariff.Name,
                    Description = tariff.Description,
                    InterestRate = tariff.InterestRate,
                    OfficerId = tariff.OfficerId,
                })
            }
        };
    }

    public override async Task<CreateTariffReply> CreateTariff(CreateTariffRequest request,
        ServerCallContext context)
    {
        await FirebaseUtil.Validate(request.Token);
        var userId = await FirebaseUtil.GetUserId(request.Token);

        var httpClient = _httpClientFactory.CreateClient(Constants.LoanHttpClient);
        var content = new NewTariff(
            Name: request.Name,
            Description: request.Description,
            InterestRate: request.InterestRate,
            OfficerId: userId
        );

        var response = await httpClient.PostAsJsonAsync("tariff", content);
        if (!response.IsSuccessStatusCode)
            _logger.LogInformation("CreateTariff FAILED: {Response}", response.ToString());
        response.EnsureSuccessStatusCode();

        var tariff = await response.Content.ReadFromJsonAsync<Dto.Tariff>();
        if (tariff == null)
            throw new Exception("CreateTariff FAILED: tariff == null");

        return new CreateTariffReply
        {
            Tariff = new Tariff
            {
                Id = tariff.Id,
                AdditionDate = tariff.AdditionDate,
                Name = tariff.Name,
                Description = tariff.Description,
                InterestRate = tariff.InterestRate,
                OfficerId = tariff.OfficerId,
            }
        };
    }
}