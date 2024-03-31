namespace trb_officer_backend.Dto;

public record PageUser(
    int TotalPages,
    int TotalElements,
    PageableObject Pageable,
    int Size,
    List<User> Content,
    int Number,
    SortObject Sort,
    int NumberOfElements,
    bool First,
    bool Last,
    bool Empty
);