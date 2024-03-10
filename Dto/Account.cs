namespace trb_officer_backend.Dto;

public record Account(
    string Id,
    string Type,
    long Balance,
    string ClientFullName,
    string ExternalClientId,
    long CreationDate,
    long? ClosingDate,
    bool IsClosed
);