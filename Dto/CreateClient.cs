namespace trb_officer_backend.Dto;

public record CreateClient
(
    string FirstName,
    string LastName,
    string? Patronymic,
    string BirthDate,
    string PhoneNumber,
    string Address,
    string PassportNumber,
    string? PassportSeries,
    string WhoCreated,
    string Email,
    string Password,
    string Sex
);