namespace trb_officer_backend.Dto;

public record PageableObject
(
    int PageNumber,
    int PageSize,
    long Offset,
    SortObject Sort,
    bool Paged,
    bool Unpaged
);