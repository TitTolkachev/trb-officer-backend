namespace trb_officer_backend.Dto;

public record PageOfficer(
    int TotalPages,
    int TotalElements,
    PageableObject Pageable,
    int Size,
    List<Officer> Content,
    int Number,
    SortObject Sort,
    int NumberOfElements,
    bool First,
    bool Last,
    bool Empty
);