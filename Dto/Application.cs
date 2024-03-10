namespace trb_officer_backend.Dto;

public record Application(
    string Id,
    long CreationDate,
    long? UpdatedDateFinal,
    int LoanTermInDays,
    long IssuedAmount,
    string ClientId,
    string? OfficerId,
    string State,
    Tariff Tariff
);