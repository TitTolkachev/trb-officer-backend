namespace trb_officer_backend.Dto;

public record CreditRating(
    string Id,
    string ClientId,
    long CalculationDate,
    int Rating
);