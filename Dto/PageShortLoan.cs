namespace trb_officer_backend.Dto;

public record PageShortLoan(
    int TotalPages,
    long TotalElements,
    int Size,
    List<LoanShort> Content,
    int Number,
    SortObject Sort,
    int NumberOfElements,
    PageableObject Pageable,
    bool First,
    bool Last,
    bool Empty
);