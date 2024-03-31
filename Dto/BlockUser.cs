namespace trb_officer_backend.Dto;

public record BlockUser(
    string UserId,
    string WhoBlocksId
);