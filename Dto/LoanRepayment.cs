namespace trb_officer_backend.Dto;

public record LoanRepayment(
    string Id,
    long Date,
    long Amount,
    string State
);