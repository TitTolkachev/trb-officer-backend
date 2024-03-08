using System.Collections.Immutable;
using Grpc.Core;
using trb_officer_backend.Common;
using trb_officer_backend.Dto;
using trb_officer_backend.Dto.Request;

namespace trb_officer_backend.Services;

public class UserServiceImpl : UserService.UserServiceBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<UserServiceImpl> _logger;

    public UserServiceImpl(IHttpClientFactory httpClientFactory, ILogger<UserServiceImpl> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public override async Task<GetClientListReply> GetClientList(GetClientListRequest request,
        ServerCallContext context)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.UserHttpClient);
        var content = new Page(PageNumber: 0, PageSize: 1000);

        var response = await httpClient.PostAsJsonAsync("users/client-page", content);
        _logger.LogInformation("GetClientList FAILED: {Response}", response.ToString());
        response.EnsureSuccessStatusCode();
        var page = await response.Content.ReadFromJsonAsync<PageClient>();

        if (page == null)
            return new GetClientListReply { Clients = { ImmutableList<UserShort>.Empty } };

        var reply = new GetClientListReply
        {
            Clients =
            {
                page.Content.ConvertAll(c => new UserShort
                    { Id = c.Id, FirstName = c.FirstName, LastName = c.LastName, BirthDate = c.BirthDate })
            }
        };

        return reply;
    }

    public override async Task<GetOfficerListReply> GetOfficerList(GetOfficerListRequest request,
        ServerCallContext context)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.UserHttpClient);
        var content = new Page(PageNumber: 0, PageSize: 1000);

        var response = await httpClient.PostAsJsonAsync("users/officer-page", content);
        _logger.LogInformation("GetOfficerList FAILED: {Response}", response.ToString());
        response.EnsureSuccessStatusCode();
        var page = await response.Content.ReadFromJsonAsync<PageOfficer>();

        if (page == null)
            return new GetOfficerListReply { Officers = { ImmutableList<UserShort>.Empty } };

        var reply = new GetOfficerListReply
        {
            Officers =
            {
                page.Content.ConvertAll(c => new UserShort
                    { Id = c.Id, FirstName = c.FirstName, LastName = c.LastName, BirthDate = c.BirthDate })
            }
        };

        return reply;
    }

    public override async Task<CreateClientReply> CreateClient(CreateClientRequest request,
        ServerCallContext context)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.UserHttpClient);
        var content = new CreateClient(
            FirstName: request.FirstName,
            LastName: request.LastName,
            Patronymic: request.Patronymic,
            BirthDate: FromMs(request.BirthDate).ToString("yyyy-MM-dd"),
            PhoneNumber: request.PhoneNumber,
            Address: request.Address,
            PassportNumber: request.PassportNumber,
            PassportSeries: request.PassportSeries,
            WhoCreated: request.WhoCreatedId,
            Email: request.Email,
            Password: request.Password,
            Sex: request.Sex);
        
        var response = await httpClient.PostAsJsonAsync("users/create-client", content);
        _logger.LogInformation("CreateClient FAILED: {Response}", response.ToString());
        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<Dto.Client>();
        
        if (user == null)
            return new CreateClientReply { Error = "Not Created" };
    
        var reply = new CreateClientReply { Id = user.Id };
    
        return reply;
    }

    public override async Task<CreateOfficerReply> CreateOfficer(CreateOfficerRequest request,
        ServerCallContext context)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.UserHttpClient);
        var content = new CreateOfficer(
            FirstName: request.FirstName,
            LastName: request.LastName,
            Patronymic: request.Patronymic,
            BirthDate: FromMs(request.BirthDate).ToString("yyyy-MM-dd"),
            PhoneNumber: request.PhoneNumber,
            Address: request.Address,
            PassportNumber: request.PassportNumber,
            PassportSeries: request.PassportSeries,
            WhoCreated: request.WhoCreatedId,
            Email: request.Email,
            Password: request.Password,
            Sex: request.Sex);
        
        var response = await httpClient.PostAsJsonAsync("users/create-officer", content);
        _logger.LogInformation("CreateOfficer FAILED: {Response}", response.ToString());
        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<Dto.Officer>();
        
        if (user == null)
            return new CreateOfficerReply { Error = "Not Created" };
    
        var reply = new CreateOfficerReply { Id = user.Id };
    
        return reply;
    }
    
    private static DateTime FromMs(long milliSec)
    {
        var startTime = new DateTime(1970, 1, 1);
        var time = TimeSpan.FromMilliseconds(milliSec);
        return startTime.Add(time);
    }
}