namespace trb_officer_backend.Dto;

public record LoanShort(
    string Id,
    long IssuedDate,
    long RepaymentDate,
    long AmountDebt,
    double InterestRate
);