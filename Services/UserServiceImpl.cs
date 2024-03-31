using Grpc.Core;
using trb_officer_backend.Common;
using trb_officer_backend.Dto;

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
        await FirebaseUtil.Validate(request.Token);

        var httpClient = _httpClientFactory.CreateClient(Constants.UserHttpClient);
        var content = new Page(PageNumber: 0, PageSize: 1000);

        var response = await httpClient.PostAsJsonAsync("users/client-page", content);
        if (!response.IsSuccessStatusCode)
            _logger.LogInformation("GetClientList FAILED: {Response}", response.ToString());
        response.EnsureSuccessStatusCode();

        var page = await response.Content.ReadFromJsonAsync<PageUser>();
        if (page == null)
            throw new Exception("GetClientList FAILED: page == null");

        return new GetClientListReply
        {
            Clients =
            {
                page.Content.FindAll(it=>it.IsClient).ConvertAll(c => new UserShort
                    { Id = c.Id, FirstName = c.FirstName, LastName = c.LastName, BirthDate = c.BirthDate })
            }
        };
    }

    public override async Task<GetOfficerListReply> GetOfficerList(GetOfficerListRequest request,
        ServerCallContext context)
    {
        await FirebaseUtil.Validate(request.Token);

        var httpClient = _httpClientFactory.CreateClient(Constants.UserHttpClient);
        var content = new Page(PageNumber: 0, PageSize: 1000);

        var response = await httpClient.PostAsJsonAsync("users/client-page", content);
        if (!response.IsSuccessStatusCode)
            _logger.LogInformation("GetOfficerList FAILED: {Response}", response.ToString());
        response.EnsureSuccessStatusCode();

        var page = await response.Content.ReadFromJsonAsync<PageUser>();
        if (page == null)
            throw new Exception("GetOfficerList FAILED: page == null");

        var reply = new GetOfficerListReply
        {
            Officers =
            {
                page.Content.FindAll(it=>it.IsOfficer).ConvertAll(c => new UserShort
                    { Id = c.Id, FirstName = c.FirstName, LastName = c.LastName, BirthDate = c.BirthDate })
            }
        };

        return reply;
    }

    public override async Task<GetClientReply> GetClient(GetClientRequest request,
        ServerCallContext context)
    {
        await FirebaseUtil.Validate(request.Token);

        var httpClient = _httpClientFactory.CreateClient(Constants.UserHttpClient);

        var response = await httpClient.GetAsync($"users/client-info?clientId={request.ClientId}");
        if (!response.IsSuccessStatusCode)
            _logger.LogInformation("GetClient FAILED: {Response}", response.ToString());
        response.EnsureSuccessStatusCode();

        var user = await response.Content.ReadFromJsonAsync<User>();
        if (user == null)
            throw new Exception("GetClient FAILED: user == null");

        var reply = new GetClientReply
        {
            Client = new Client
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = Util.ToMs(user.BirthDate),
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                PassportNumber = user.PassportNumber,
                Email = user.Email,
                Sex = user.Sex,
                Blocked = user.Blocked
            }
        };
        if (user.Patronymic != null)
            reply.Client.Patronymic = user.Patronymic;
        if (user.PassportSeries != null)
            reply.Client.PassportSeries = user.PassportSeries;
        if (user.WhoBlocked != null)
            reply.Client.WhoBlocked = user.WhoBlocked.Id;
        if (user.WhoCreated != null)
            reply.Client.WhoCreated = user.WhoCreated.Id;

        return reply;
    }

    public override async Task<GetOfficerReply> GetOfficer(GetOfficerRequest request,
        ServerCallContext context)
    {
        await FirebaseUtil.Validate(request.Token);

        var httpClient = _httpClientFactory.CreateClient(Constants.UserHttpClient);

        var response = await httpClient.GetAsync($"users/client-info?clientId={request.OfficerId}");
        if (!response.IsSuccessStatusCode)
            _logger.LogInformation("GetOfficer FAILED: {Response}", response.ToString());
        response.EnsureSuccessStatusCode();

        var user = await response.Content.ReadFromJsonAsync<User>();
        if (user == null)
            throw new Exception("GetOfficer FAILED: user == null");

        var reply = new GetOfficerReply
        {
            Officer = new Officer
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = Util.ToMs(user.BirthDate),
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                PassportNumber = user.PassportNumber,
                Email = user.Email,
                Sex = user.Sex,
                Blocked = user.Blocked
            }
        };
        if (user.Patronymic != null)
            reply.Officer.Patronymic = user.Patronymic;
        if (user.PassportSeries != null)
            reply.Officer.PassportSeries = user.PassportSeries;
        if (user.WhoBlocked != null)
            reply.Officer.WhoBlocked = user.WhoBlocked.Id;
        if (user.WhoCreated != null)
            reply.Officer.WhoCreated = user.WhoCreated.Id;

        return reply;
    }

    public override async Task<BlockUserReply> BlockUser(BlockUserRequest request,
        ServerCallContext context)
    {
        await FirebaseUtil.Validate(request.Token);
        var userId = await FirebaseUtil.GetUserId(request.Token);

        var httpClient = _httpClientFactory.CreateClient(Constants.UserHttpClient);
        var content = new BlockUser(UserId: request.UserId, WhoBlocksId: userId);
        
        var response = await httpClient.PostAsJsonAsync("users/block-user", content);
        if (!response.IsSuccessStatusCode)
            _logger.LogInformation("BlockClient FAILED: {Response}", response.ToString());
        response.EnsureSuccessStatusCode();
        
        return new BlockUserReply();
    }

    public override async Task<CreateUserReply> CreateUser(CreateUserRequest request,
        ServerCallContext context)
    {
        await FirebaseUtil.ValidateWithId(request.Token, request.WhoCreatedId);

        var httpClient = _httpClientFactory.CreateClient(Constants.UserHttpClient);
        var content = new CreateUser(
            FirstName: request.FirstName,
            LastName: request.LastName,
            Patronymic: request.Patronymic,
            BirthDate: Util.FromMs(request.BirthDate).ToString("yyyy-MM-dd"),
            PhoneNumber: request.PhoneNumber,
            Address: request.Address,
            PassportNumber: request.PassportNumber,
            PassportSeries: request.PassportSeries,
            WhoCreated: request.WhoCreatedId,
            Email: request.Email,
            Password: request.Password,
            Sex: request.Sex,
            IsClient: request.IsClient,
            IsOfficer: request.IsOfficer);

        var response = await httpClient.PostAsJsonAsync("users/create-user", content);
        if (!response.IsSuccessStatusCode)
            _logger.LogInformation("CreateClient FAILED: {Response}", response.ToString());
        response.EnsureSuccessStatusCode();

        var user = await response.Content.ReadFromJsonAsync<User>();
        if (user == null)
            throw new Exception("CreateClient FAILED: user == null");

        return new CreateUserReply { Id = user.Id };
    }
}