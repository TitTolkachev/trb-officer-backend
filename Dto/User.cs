namespace trb_officer_backend.Dto;

public record User(
    string Id,
    string FirstName,
    string LastName,
    string? Patronymic,
    string BirthDate,
    string Email,
    string PhoneNumber,
    bool IsClient,
    bool IsOfficer,
    string Address,
    string PassportNumber,
    string? PassportSeries,
    User? WhoBlocked,
    User? WhoCreated,
    string Sex,
    bool Blocked
);