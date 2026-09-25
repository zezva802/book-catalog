using BookCatalog.Api.Entities;
using BookCatalog.Api.Interfaces;

namespace BookCatalog.Api.Storage;
public class BookStorage : IBookStorage
{
    private readonly Dictionary<int, Book> _books = new();
    private int _counter = 0;

    private readonly object _lock = new();


    public Book Create(string title, string? author, int? year)
    {   
        lock (_lock)
        {
            _counter +=1;
            Book book = new()
            {
            Id = _counter,
            Title = title,
            Author = author,
            Year = year
            };
            _books.Add(_counter, book);
            return book;
        }
        
    }

    public void Delete(int id)
    {
        lock (_lock)
        {
            _books.Remove(id);
        }
        
    }

    public IEnumerable<Book> GetAll()
    {   
        lock (_lock)
        {
            return _books.Values;
        }
    }

    public Book? GetById(int id)
    {
        lock (_lock)
        {
            if (_books.TryGetValue(id, out var book))
            {
                return book;
            }
            return null;
        }
        
    }

    public Book? Update(int id, Book book)
    {   
        lock (_lock)
        {
            if (_books.ContainsKey(id))
            {
                book.Id = id;
                _books[id] = book;
                return book;
            }
            return null;
        }
    }
}