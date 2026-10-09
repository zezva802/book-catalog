using BookCatalog.Domain.Entities;

namespace BookCatalog.Application.Abstractions;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Book> CreateAsync(Book book, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Book book, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}