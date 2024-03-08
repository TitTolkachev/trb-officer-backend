namespace trb_officer_backend.Dto;

public record NewTariff(
    string Name,
    string Description,
    double InterestRate,
    string OfficerId
);