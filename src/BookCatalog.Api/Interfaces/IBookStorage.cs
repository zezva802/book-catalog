using BookCatalog.Api.Entities;

namespace BookCatalog.Api.Interfaces;

public interface IBookStorage
{
    Book? GetById(int id);

    IEnumerable<Book> GetAll();

    Book Create(string title, string? author, int? year);

    Book? Update(int id, Book book);

    bool Delete(int id);

}