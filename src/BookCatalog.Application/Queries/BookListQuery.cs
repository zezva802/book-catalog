namespace BookCatalog.Application.Queries;
public record BookListQuery(string? Title, string? Author, int? YearFrom, int? YearTo, int Page, int PageSize);