using System.Collections.Immutable;
using Grpc.Core;
using trb_officer_backend.Common;
using trb_officer_backend.Dto;
using trb_officer_backend.Dto.Request;

namespace trb_officer_backend.Services;

public class UserServiceImpl : UserService.UserServiceBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public UserServiceImpl(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public override async Task<GetClientListReply> GetClientList(GetClientListRequest request,
        ServerCallContext context)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.UserHttpClient);
        var content = new Page(PageNumber: 0, PageSize: 10);

        var response = await httpClient.PostAsJsonAsync("users/client-page", content);
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
        var content = new Page(PageNumber: 0, PageSize: 10);

        var response = await httpClient.PostAsJsonAsync("users/officer-page", content);
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

    // public override async Task<CreateClientReply> CreateClient(CreateClientRequest request,
    //     ServerCallContext context)
    // {
    //     var httpClient = _httpClientFactory.CreateClient("Main");
    //     var content = new CreateClient(
    //         FirstName: request.FirstName,
    //         LastName: request.LastName,
    //          Patronymic: request.Patronymic,
    //         BirthDate: ,
    //         PhoneNumber: request.PhoneNumber,
    //         Address: request.Address,
    //         PassportNumber: request.PassportNumber,
    //          PassportSeries: request.PassportSeries,
    //         WhoCreated: ,
    //         Email: request.Email,
    //         Password: request.Password,
    //         Sex: request.
    //         );
    //
    //
    //     var response = await httpClient.PostAsJsonAsync("users/client-page", content);
    //     response.EnsureSuccessStatusCode();
    //     var pageClient = await response.Content.ReadFromJsonAsync<PageClient>();
    //
    //
    //     if (pageClient == null)
    //         return new CreateClientReply { Clients = { ImmutableList<UserShort>.Empty } };
    //
    //     var reply = new GetClientListReply
    //     {
    //         Clients =
    //         {
    //             pageClient.Content.ConvertAll(c => new UserShort
    //                 { Id = c.Id, FirstName = c.FirstName, LastName = c.LastName, BirthDate = c.BirthDate })
    //         }
    //     };
    //
    //     return reply;
    // }
}