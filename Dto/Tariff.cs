namespace trb_officer_backend.Dto;

public record Tariff(
    string Id,
    string AdditionDate,
    string Name,
    string Description,
    double InterestRate,
    string OfficerId
);