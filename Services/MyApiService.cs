using Grpc.Core;

namespace trb_officer_backend.Services;

public class MyApiService : ApiService.ApiServiceBase
{
    private readonly ILogger<MyApiService> _logger;

    public MyApiService(ILogger<MyApiService> logger)
    {
        _logger = logger;
    }

    public override Task<CreateClientReply> CreateClient(CreateClientRequest request, ServerCallContext context)
    {
        return base.CreateClient(request, context);
    }

    public override Task<GetClientListReply> GetClientList(GetClientListRequest request, ServerCallContext context)
    {
        return Task.FromResult(new GetClientListReply
        {
            Clients =
            {
                new List<UserShort>
                {
                    new() { Id = "1", FirstName = "Name 1", LastName = "LastName 1", BirthDate = "11.11.2011" },
                    new() { Id = "2", FirstName = "Name 2", LastName = "LastName 2", BirthDate = "11.11.2011" },
                    new() { Id = "3", FirstName = "Name 3", LastName = "LastName 3", BirthDate = "11.11.2011" }
                }
            }
        });
    }
}