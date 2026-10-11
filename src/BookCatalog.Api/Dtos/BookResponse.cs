namespace BookCatalog.Api.Dtos;

public record BookResponse(int Id, string Title, string? Author, int? Year);