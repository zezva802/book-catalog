using BookCatalog.Application.Abstractions;
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

    public Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<Book>>(_books.Values.ToList());
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
            return Task.FromResult<Book>(book);
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
                return Task.FromResult<bool>(true);
            }
            return Task.FromResult<bool>(false);
        }
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {   
        cancellationToken.ThrowIfCancellationRequested();
        lock (_lock)
        {
            return Task.FromResult<bool>(_books.Remove(id));
        }
    }
}