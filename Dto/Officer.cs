namespace trb_officer_backend.Dto;

public record Officer(
    string Id,
    string FirstName,
    string LastName,
    string? Patronymic,
    string BirthDate,
    string Email,
    string PhoneNumber,
    string Address,
    string PassportNumber,
    string? PassportSeries,
    Officer? WhoBlocked,
    Officer? WhoCreated,
    string Sex,
    bool Blocked
);