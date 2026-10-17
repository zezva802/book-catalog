using BookCatalog.Application.Abstractions;
using BookCatalog.Application.Common;
using BookCatalog.Application.Queries;
using BookCatalog.Domain.Entities;

namespace BookCatalog.Infrastructure;

public class InMemoryBookRepository : IBookRepository
{
    private readonly Dictionary<int, Book> _books = new();
    private int _counter = 0;
    private readonly object _lock = new();

    public Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_lock)
        {
            _books.TryGetValue(id, out var book);
            return Task.FromResult<Book?>(book);
        }
    }


    public Task<PagedResult<Book>> ListAsync(BookListQuery query, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_lock)
        {
            IEnumerable<Book> books = ApplyFilters(_books.Values, query);

            var totalCount = books.Count();

            var items = books
                    .OrderBy(b => b.Title).ThenBy(b => b.Id)
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToList();


            return Task.FromResult(new PagedResult<Book>(items, totalCount, query.Page, query.PageSize));
        }
    }

    public Task<Book> CreateAsync(Book book, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_lock)
        {
            _counter += 1;
            book.Id = _counter;
            _books.Add(_counter, book);
            return Task.FromResult(book);
        }
    }

    public Task<bool> UpdateAsync(Book book, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_lock)
        {
            if (_books.ContainsKey(book.Id))
            {
                _books[book.Id] = book;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_lock)
        {
            return Task.FromResult(_books.Remove(id));
        }
    }

    private static IEnumerable<Book> ApplyFilters(IEnumerable<Book> books, BookListQuery query)
    {
        if (query.Title is not null)
        {
            books = books.Where(b => b.Title.Contains(query.Title, StringComparison.OrdinalIgnoreCase));
        }

        if (query.Author is not null)
        {
            books = books.Where(b => b.Author is not null && b.Author.Contains(query.Author, StringComparison.OrdinalIgnoreCase));
        }

        if (query.YearFrom is not null)
        {
            books = books.Where(b => b.Year >= query.YearFrom);
        }

        if (query.YearTo is not null)
        {
            books = books.Where(b => b.Year <= query.YearTo);
        }

        return books;
    }
}