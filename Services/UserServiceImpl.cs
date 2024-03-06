using System.Collections.Immutable;
using Grpc.Core;
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
        var httpClient = _httpClientFactory.CreateClient("Main");
        var content = new Page(PageNumber: 1, PageSize: 10);


        var response = await httpClient.PostAsJsonAsync("users/client-page", content);
        response.EnsureSuccessStatusCode();
        var pageClient = await response.Content.ReadFromJsonAsync<PageClient>();


        if (pageClient == null)
            return new GetClientListReply { Clients = { ImmutableList<UserShort>.Empty } };

        var reply = new GetClientListReply
        {
            Clients =
            {
                pageClient.Content.ConvertAll(c => new UserShort
                    { Id = c.Id, FirstName = c.FirstName, LastName = c.LastName, BirthDate = c.BirthDate })
            }
        };

        return reply;
    }
}