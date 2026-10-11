using BookCatalog.Domain.Entities;

namespace BookCatalog.Api.Dtos;

public static class BookMappings
{
    public static Book ToEntity(this BookRequest request) => new Book
    {
        Title = request.Title,
        Author=request.Author,
        Year=request.Year
    };

    public static Book ToEntity(this BookRequest request, int id) => new Book
    {
        Id=id,
        Title=request.Title,
        Author=request.Author,
        Year=request.Year
    };

    public static BookResponse ToResponse(this Book book) =>
        new(book.Id, book.Title, book.Author, book.Year);
}