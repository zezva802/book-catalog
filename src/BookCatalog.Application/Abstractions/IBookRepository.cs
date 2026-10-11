using BookCatalog.Application.Common;
using BookCatalog.Application.Queries;
using BookCatalog.Domain.Entities;

namespace BookCatalog.Application.Abstractions;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<Book>> ListAsync(BookListQuery query, CancellationToken cancellationToken = default);
    Task<Book> CreateAsync(Book book, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Book book, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}