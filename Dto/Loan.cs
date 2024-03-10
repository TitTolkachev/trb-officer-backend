namespace trb_officer_backend.Dto;

public record Loan(
    string Id,
    long IssuedDate,
    long RepaymentDate,
    long IssuedAmount,
    long AmountDebt,
    long AccruedPenny,
    long LoanTermInDays,
    string ClientId,
    string AccountId,
    string State,
    Tariff Tariff,
    List<LoanRepayment> Repayments
);