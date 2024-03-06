namespace trb_officer_backend.Dto;

public record PageClient(
    int TotalPages,
    int TotalElements,
    PageableObject Pageable,
    int Size,
    List<Client> Content,
    int Number,
    SortObject Sort,
    int NumberOfElements,
    bool First,
    bool Last,
    bool Empty
);