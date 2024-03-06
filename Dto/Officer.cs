namespace trb_officer_backend.Dto;

public record Officer(
    string Id,
    string FirstName,
    string LastName,
    string? BirthDate,
    string Email,
    string Password,
    string PhoneNumber,
    string Address,
    string PassportNumber,
    string? PassportSeries,
    Officer? WhoBlocked,
    Officer? WhoCreated,
    Sex Sex,
    bool Blocked
);