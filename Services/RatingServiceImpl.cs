using Grpc.Core;
using trb_officer_backend.Common;

namespace trb_officer_backend.Services;

public class RatingServiceImpl : RatingService.RatingServiceBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<RatingServiceImpl> _logger;

    public RatingServiceImpl(IHttpClientFactory httpClientFactory, ILogger<RatingServiceImpl> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public override async Task<UpdateUserRatingResponse> UpdateUserRating(UpdateUserRatingRequest request,
        ServerCallContext context)
    {
        await FirebaseUtil.Validate(request.Token);

        var httpClient = _httpClientFactory.CreateClient(Constants.LoanHttpClient);

        var response = await httpClient.PostAsync($"credit-ratings?clientId={request.ClientId}", null);
        if (!response.IsSuccessStatusCode)
            _logger.LogInformation("UpdateUserRating FAILED: {Response}", response.ToString());
        response.EnsureSuccessStatusCode();

        var rating = await response.Content.ReadFromJsonAsync<Dto.CreditRating>();
        if (rating == null)
            throw new Exception("UpdateUserRating FAILED: rating == null");

        return new UpdateUserRatingResponse
        {
            Rating = new CreditRating {
                CalculationDate = rating.CalculationDate,
                Rating = rating.Rating
            }
        };
    }

    public override async Task<GetUserRatingResponse> GetUserRating(GetUserRatingRequest request,
        ServerCallContext context)
    {
        await FirebaseUtil.Validate(request.Token);

        var httpClient = _httpClientFactory.CreateClient(Constants.LoanHttpClient);

        var response = await httpClient.GetAsync($"credit-ratings/last?clientId={request.ClientId}");
        if (!response.IsSuccessStatusCode)
            _logger.LogInformation("GetUserRating FAILED: {Response}", response.ToString());
        response.EnsureSuccessStatusCode();

        var rating = await response.Content.ReadFromJsonAsync<Dto.CreditRating>();
        if (rating == null)
            throw new Exception("GetUserRating FAILED: rating == null");

        return new GetUserRatingResponse
        {
            Rating = new CreditRating {
                CalculationDate = rating.CalculationDate,
                Rating = rating.Rating
            }
        };
    }
}