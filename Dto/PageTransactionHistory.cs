namespace trb_officer_backend.Dto;

public record PageTransactionHistory(
    int PageNumber,
    int PageSize,
    List<Transaction> Elements
);